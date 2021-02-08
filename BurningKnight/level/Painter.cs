using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.assets.items;
using BurningKnight.assets.prefabs;
using BurningKnight.entity;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.door;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.room;
using BurningKnight.entity.room.controllable;
using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.entity.room.input;
using BurningKnight.level.biome;
using BurningKnight.level.entities;
using BurningKnight.level.entities.decor;
using BurningKnight.level.entities.plant;
using BurningKnight.level.paintings;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.connection;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.rooms.secret;
using BurningKnight.level.rooms.treasure;
using BurningKnight.level.tile;
using BurningKnight.level.walls;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level {
	public class Painter {
		public static bool AllGold;

		public float Cobweb = 0.2f;
		public float Dirt = 0.4f;
		public float Grass = 0.4f;
		public float Water = 0.4f;
		public float Fireflies = 1f;
		public float FirefliesChance = 60f;
		public List<Action<Level, RoomDef>> RoomModifiers = new List<Action<Level, RoomDef>>();
		public List<Action<Level, RoomDef, int, int>> Modifiers = new List<Action<Level, RoomDef, int, int>>();
		public static Rect Clip;
		public Tile DirtTile = Tile.Dirt;
		
		public Painter() {
			AllGold = false;
			
			// All the rocks, that have not full neighbours will become metal blocks (33% chance)
			RoomModifiers.Add((l, r) => {
				if (Rnd.Chance(66)) {
					return;
				}
				
				for (var y = r.Top; y <= r.Bottom; y++) {
					for (var x = r.Left; x <= r.Right; x++) {
						var index = l.ToIndex(x, y);

						if (l.Get(index, true) == Tile.Rock) {
							var sum = 0;

							foreach (var dir in PathFinder.Neighbours8) {
								var n = dir + index;

								if (l.IsInside(n) && (TileFlags.Matches(l.Tiles[n], TileFlags.Solid) ||
								                      TileFlags.Matches(l.Liquid[n], TileFlags.HalfWall))) {
									sum++;
								}
							}

							if (sum > 0 && sum < 8) {
								l.Set(index, Tile.MetalBlock);

								return;
							}
						}
					}
				}
			});
			
			// Small chance to replace rock with a tinted rock or with a barrel
			Modifiers.Add((l, rm, x, y) => {
				var index = l.ToIndex(x, y);

				if (l.Get(index, true) == Tile.Rock) {
					var r = Rnd.Float();

					if (r <= 0.05f) {
						l.Set(index, Tile.TintedRock);
					} else if (Run.Depth > 0 && r <= 0.1f && !(rm is TreasureRoom)) {
						l.Set(index, Tile.BarrelTmp);
					}
				}
			});
		}
		
		private void InspectRoom(RoomDef room) {
			foreach (var r in room.Connected.Keys) {
				if (r.Distance == -1) {
					r.Distance = room.Distance + 1;
					InspectRoom(r);
				}
			}
		}

		public bool Paint(Level Level, List<RoomDef> Rooms) {
			if (Rooms == null) {
				return false;
			}

			Level.Rains = Run.Depth > 0 && Rnd.Chance(15);

			if (Level.Biome.Id == Biome.Ice) {
				Level.Snows = true;
				Level.Rains = false;
			} else if (Level.Biome.Id == Biome.Castle) {
				Level.Snows = Run.Depth > 0 && Rnd.Chance(10);
			} else if (Level.Biome.Id == Biome.Desert) {
				Level.Rains = false;
			}

			// Level.Dark = Run.Depth > 1 && Rnd.Chance(5);

			if (Run.Depth == 5 && LevelSave.GenerateMarket && Run.Loop == 0) {
				Level.Dark = true;
			}
			
			RoomDef current = null;

			foreach (var r in Rooms) {
				if (r is ExitRoom) {
					current = r;
					break;
				}
			}

			if (current == null) {
				Log.Error("Failed to find the exit room");
			} else {
				current.Distance = 0;
				InspectRoom(current);
			}

			var LeftMost = int.MaxValue;
			var TopMost = int.MaxValue;

			foreach (var R in Rooms) {
				if (R.Left < LeftMost) {
					LeftMost = R.Left;
				}

				if (R.Top < TopMost) {
					TopMost = R.Top;
				}
			}

			LeftMost--;
			TopMost--;
			var Sz = Level.GetPadding();
			LeftMost -= Sz;
			TopMost -= Sz;
			var RightMost = 0;
			var BottomMost = 0;

			foreach (var R in Rooms) {
				R.Shift(-LeftMost, -TopMost);

				if (R.Right > RightMost) {
					RightMost = R.Right;
				}

				if (R.Bottom > BottomMost) {
					BottomMost = R.Bottom;
				}
			}

			RightMost++;
			BottomMost++;
			RightMost += Sz;
			BottomMost += Sz;
			
			Log.Info($"Setting level size to {(1 + RightMost)}:{(BottomMost + 1)}");
			
			Level.Width = RightMost + 1;
			Level.Height = BottomMost + 1;
			
			Level.Setup();

			var tile = Level.GetFilling();
			var liquid = tile.Matches(TileFlags.LiquidLayer);

			if (liquid) {
				var t = (byte) Tile.FloorA;
				
				for (int i = 0; i < Level.Size; i++) {
					Level.Tiles[i] = t;
					Level.Liquid[i] = (byte) tile;
				}
			} else {
				for (int i = 0; i < Level.Size; i++) {
					Level.Tiles[i] = (byte) tile;
				}	
			}

			if (Level.Biome is IceBiome || (Run.Depth > 0 && tile == Tile.Chasm)) {
				var z = Level.Biome is IceBiome ? Tile.WallB : Tile.WallA;
				
				for (var x = 0; x < Level.Width; x++) {
					for (var y = 0; y < 5; y++) {
						if (y == 0 || Rnd.Chance((5 - y) * 20)) {
							Set(Level, x, y, z);
						}
					}
					
					for (var y = Level.Height - 6; y < Level.Height; y++) {
						if (y == Level.Height - 1 || Rnd.Chance((y - Level.Height + 5) * 20)) {
							Set(Level, x, y, z);
						}
					}
				}
				
				for (var y = 0; y < Level.Height; y++) {
					for (var x = 0; x < 5; x++) {
						if (x == 0 || Rnd.Chance((5 - x) * 20)) {
							Set(Level, x, y, z);
						}
					}
					
					for (var x = Level.Width - 6; x < Level.Width; x++) {
						if (x == Level.Width - 1 || Rnd.Chance((x - Level.Width + 5) * 20)) {
							Set(Level, x, y, z);
						}
					}
				}
			}

			var tr = Level.GetFilling();
			RoomDef exit = null;
			RoomDef entrance = null;

			for (var i = Rooms.Count - 1; i >= 0; i--) {
				var Room = Rooms[i];
				PlaceDoors(Room);

				if (Room is EntranceRoom) {
					entrance = Room;
				} else if (Room is ExitRoom) {
					exit = Room;
				}

				foreach (var d in Room.Connected.Values) {
					if (d.Type != DoorPlaceholder.Variant.Empty && d.Type != DoorPlaceholder.Variant.Secret &&
					    d.Type != DoorPlaceholder.Variant.Maze) {

						if (d.X == Room.Left || d.X == Room.Right) {
							Set(Level, d.X, d.Y - 1, tr);
							Set(Level, d.X, d.Y + 1, tr);
						} else {
							Set(Level, d.X - 1, d.Y, tr);
							Set(Level, d.X + 1, d.Y, tr);
						}
					}
				}
			}

			// Not checking lib cuz teleporters
			var check = Run.Depth > 0 && !(LevelSave.BiomeGenerated is LibraryBiome);

			if (check && (exit == null || entrance == null)) {
				Log.Error("Exit or entrance not found, aborting");
				return false;
			}

			var fl = Level.GetFilling() == Tile.WallB ? Tile.WallB : Tile.WallA;

			for (var i = Rooms.Count - 1; i >= 0; i--) {
				var Room = Rooms[i];
			
				if (!(Room is ConnectionRoom)) {
					Rect(Level, Room, 0, LevelSave.BiomeGenerated is IceBiome ? Tile.WallB : fl);
				}

				Clip = Room.Shrink();

				Room.PaintFloor(Level);
				Room.Paint(Level);

				if (Room is SecretRoom) {
					for (var Y = Room.Top + 1; Y < Room.Bottom; Y++) {
						for (var X = Room.Left + 1; X < Room.Right; X++) {
							if (Rnd.Chance(Run.Depth * 5)) {
								Level.MatrixLeak[Level.ToIndex(X, Y)] = true;
							}
						}
					}
				}

				if (!(Room is TreasureRoom)) {
					foreach (var d in Room.Connected.Values) {
						if (d.Type != DoorPlaceholder.Variant.Secret) {
							var a = d.X == Room.Left || d.X == Room.Right;
							var w = a ? 2 : 1;
							var h = a ? 1 : 2;
							var f = Tiles.RandomFloor();

							Call(Level, d.X - w, d.Y - h, w * 2 + 1, h * 2 + 1, (x, y) => {
								if (Level.Get(x, y).Matches(TileFlags.Danger)) {
									Level.Set(x, y, f);
								}
							});
						}
					}
				}

				Clip = null;
				
				Room.SetupDoors(Level);

				foreach (var m in RoomModifiers) {
					m(Level, Room);
				}

				for (var Y = Room.Top; Y <= Room.Bottom; Y++) {
					for (var X = Room.Left; X <= Room.Right; X++) {
						foreach (var m in Modifiers) {
							m(Level, Room, X, Y);
						}
					}
				}

				ReplaceTiles(Level, Room);
			}

			PathFinder.SetMapSize(Level.Width, Level.Height);

			if (Run.Depth > -1) {
				if (Dirt > 0) {
					PaintDirt(Level, Rooms);
				}

				if (Grass > 0) {
					PaintGrass(Level, Rooms);
				}

				if (Cobweb > 0) {
					PaintCobweb(Level, Rooms);
				}

				if (Water > 0) {
					PaintWater(Level, Rooms);
				}
			}

			PaintDoors(Level, Rooms);
			Decorate(Level, Rooms);

			UpdateTransition(Level);

			if (check) {
				var c = exit.GetCenter();
				Level.CreatePassable(true);
				PathFinder.SetMapSize(Level.Width, Level.Height);
				var i1 = Level.ToIndex(c.X, c.Y);
				
				PathFinder.BuildDistanceMap(i1, Level.Passable);
				c = entrance.GetCenter();

				var i2 = Level.ToIndex(c.X, c.Y);
				if (PathFinder.Distance[i2] == Int32.MaxValue) {
					Log.Error("Generated unpassable level, aborting");
				//	return false;
				}
			}

			var rooms = new List<RoomDef>();

			foreach (var rr in Rooms) {
				if (rr is RegularRoom || rr is EntranceRoom) {
					rooms.Add(rr);
				}
			}

			var rrms = new List<RoomDef>();

			foreach (var rm in rooms) {
				if (rm is RegularRoom) {
					rrms.Add(rm);
				}
			}

			if (rrms.Count > 0) {
				foreach (var type in Level.ItemsToSpawn) {
					var item = Items.CreateAndAdd(type, Level.Area);

					if (item == null) {
						continue;
					}
					
					item.Center = (rrms[Rnd.Int(rrms.Count)].GetRandomFreeCell() * 16) + new Vector2(8, 8);
				}

				if (Run.Depth == 1) {
					var crystal = new Crystal();
					crystal.Center = (rrms[Rnd.Int(rrms.Count)].GetRandomFreeCell() * 16) + new Vector2(8, 8) + Rnd.Vector(-4, 4);
					Level.Area.Add(crystal);
				}
			} else {
				Log.Error("Failed to place items");
			}

			Level.ItemsToSpawn = null;

			var rms = new List<Room>();
			
			foreach (var def in Rooms) {
				if (!def.ConvertToEntity()) {
					continue;
				}
				
				var room = new Room();

				room.Type = RoomDef.DecideType(def, def.GetType());
				room.MapX = def.Left;
				room.MapY = def.Top;
				room.MapW = def.GetWidth();
				room.MapH = def.GetHeight();
				room.Parent = def;
				
				Level.Area.Add(room);
				rms.Add(room);

				def.ModifyRoom(room);

				room.Generate();
			}
			
			PlaceMobs(Level, rms);
			return true;
		}

		public static void ReplaceTiles(Level Level, RoomDef Room) {
			for (var Y = Room.Top; Y <= Room.Bottom; Y++) {
				for (var X = Room.Left; X <= Room.Right; X++) {
					var I = Level.ToIndex(X, Y);

					var rs = !Level.Biome.HasSpikes();

					if (rs) {
						var tl = (Tile) Level.Tiles[I];

						if (tl.Matches(Tile.SensingSpikeTmp, Tile.SpikeOnTmp, Tile.SpikeOffTmp, Tile.FireTrapTmp)) {
							Level.Set(I, Tile.FloorA);
						}
					} else {
						if (Level.Tiles[I] == (byte) Tile.SensingSpikeTmp) {
							Level.Tiles[I] = (byte) Tile.FloorA;
							Level.Liquid[I] = 0;

							var spikes = new SensingSpikes();

							spikes.X = X * 16;
							spikes.Y = Y * 16;

							Level.Area.Add(spikes);
						} else if (Level.Tiles[I] == (byte) Tile.SpikeOffTmp) {
							Level.Tiles[I] = (byte) Tile.FloorA;
							Level.Liquid[I] = 0;

							var spikes = new Spikes();

							spikes.X = X * 16;
							spikes.Y = Y * 16;

							Level.Area.Add(spikes);
						} else if (Level.Tiles[I] == (byte) Tile.FireTrapTmp) {
							Level.Tiles[I] = (byte) Tile.FloorA;
							Level.Liquid[I] = 0;

							var trap = new FireTrap();

							trap.X = X * 16;
							trap.Y = Y * 16;

							Level.Area.Add(trap);
						} else if (Level.Tiles[I] == (byte) Tile.SpikeOnTmp) {
							Level.Tiles[I] = (byte) Tile.FloorA;
							Level.Liquid[I] = 0;

							var spikes = new AlwaysOnSpikes();

							spikes.X = X * 16;
							spikes.Y = Y * 16;

							Level.Area.Add(spikes);
						}
					}

				if (Level.Tiles[I] == (byte) Tile.Plate) {
						Level.Tiles[I] = (byte) Tile.FloorA;
						Level.Liquid[I] = 0;
						
						var plate = new PreasurePlate();

						plate.X = X * 16;
						plate.Y = Y * 16;

						Level.Area.Add(plate);
					} else if (Level.Tiles[I] == (byte) Tile.BarrelTmp) {
						Level.Tiles[I] = (byte) Tile.FloorA;
						Level.Liquid[I] = 0;
						
						var barrel = new ExplodingBarrel();
						Level.Area.Add(barrel);

						barrel.CenterX = X * 16 + 8;
						barrel.Bottom = Y * 16 + 16;
					}
				}
			}
		}

		public static void UpdateTransition(Level Level) {
			for (var y = 6; y < Level.Height - 5; y++) {
				for (var x = 6; x < Level.Width - 5; x++) {
					if (Level.Get(x, y) == Tile.WallA) {
						var a = (y == 0 || y == Level.Height - 1 || x == 0 || x == Level.Width - 1);

						if (!a) {
							a = true;

							foreach (var d in MathUtils.AllDirections) {
								if (!Level.Get(x + (int) d.X, y + (int) d.Y).Matches(Tile.WallA, Tile.Transition)) {
									a = false;

									break;
								}
							}
						}

						if (a) {
							Level.Set(x, y, Tile.Transition);
						}
					}
				}
			}
		}

		public static void PlaceMobs(Level level, Room room) {
			var parent = room.Parent;
			var w = parent.GetWidth() - 2;
			var h = parent.GetHeight() - 2;
			var mobs = new List<MobInfo>(MobRegistry.Current);
			
			parent.ModifyMobList(mobs);

			if (mobs.Count == 0) {
				return;
			}
			
			var chances = new List<float>();

			for (var i = 0; i < mobs.Count; i++) {
				chances.Add(parent.WeightMob(mobs[i], mobs[i].GetChanceFor(level.Biome.Id)));
			}

			var types = new List<MobInfo>();
			var spawnChances = new List<float>();
			var curseOfBlood = Scourge.IsEnabled(Scourge.OfBlood);

			if (level.Biome.SpawnAllMobs()) {
				types.AddRange(mobs);
				spawnChances.AddRange(chances);
			} else {
				for (int i = 0; i < Rnd.Int(2, 6) * (curseOfBlood ? 2 : 1); i++) {
					var ind = Rnd.Chances(chances);
					var type = mobs[ind];

					mobs.RemoveAt(ind);
					chances.RemoveAt(ind);
					types.Add(type);
					spawnChances.Add(type.Chance);

					if (mobs.Count == 0) {
						break;
					}
				}
			}

			if (types.Count == 0) {
				Log.Warning($"No mobs detected to spawn in {level.Biome.Id} biome");
				return;
			}
			
			var points = new List<Dot>();
			var wallFreePoints = new List<Dot>();
			var wallPoints = new List<Dot>();
			var size = w * h;
			var patch = new bool[size];

			Func<int, int, int> toIndex = (x, y) => (x - parent.Left - 1) + (y - parent.Top - 1) * w;

			for (var y = parent.Top + 1; y < parent.Bottom; y++) {
				for (var x = parent.Left + 1; x < parent.Right; x++) {
					patch[toIndex(x, y)] = !Run.Level.IsPassable(x, y, true);
				}
			}

			var hasDoors = parent.Connected.Count > 0;
			Dot start = null;
			
			if (hasDoors) {
				PathFinder.SetMapSize(w, h);

				var door = parent.Connected.Values.First();
				start = new Dot(door.X, door.Y);

				if ((int) start.X == parent.Left) {
					start.X++;
				} else if ((int) start.Y == parent.Top) {
					start.Y++;
				} else if ((int) start.X == parent.Right) {
					start.X--;
				} else if ((int) start.Y == parent.Bottom) {
					start.Y--;
				}

				PathFinder.BuildDistanceMap(toIndex(start.X, start.Y), BArray.Not(patch, null));
			}

			for (var y = parent.Top + 1; y < parent.Bottom; y++) {
				for (var x = parent.Left + 1; x < parent.Right; x++) {
					if (hasDoors) {
						var i = toIndex(x, y);

						if (patch[i] || PathFinder.Distance[i] == Int32.MaxValue) {
							continue;
						}

						var found = false;

						foreach (var dr in parent.Connected.Values) {
							var dx = (int) (dr.X - x);
							var dy = (int) (dr.Y - y);
							var d = (float) Math.Sqrt(dx * dx + dy * dy);

							if (d < 4) {
								found = true;

								break;
							}
						}

						if (found) {
							continue;
						}
					}

					if (room.Type == RoomType.Boss) {
						var c = parent.GetCenter();
						
						var dx = (int) (c.X - x);
						var dy = (int) (c.Y - y);
						var d = (float) Math.Sqrt(dx * dx + dy * dy);

						if (d < 3) {
							continue;
						}
					}

					var dt = new Dot(x, y);

					if ((Run.Level.IsPassable(x - 1, y) || Run.Level.IsPassable(x + 1, y)) && (Run.Level.IsPassable(x, y + 1) || Run.Level.IsPassable(x, y - 1))) {
						points.Add(dt);
						
						if (Run.Level.IsPassable(x - 1, y) && Run.Level.IsPassable(x + 1, y) && Run.Level.IsPassable(x, y + 1) && Run.Level.IsPassable(x, y - 1)) {
							wallFreePoints.Add(dt);
						}
					}

					if (Run.Level.Get(x - 1, y).IsWall() || Run.Level.Get(x + 1, y).IsWall() || Run.Level.Get(x, y - 1).IsWall() || Run.Level.Get(x , y + 1).IsWall()) {
						wallPoints.Add(dt);
					}
				}
			}

			if (points.Count + wallPoints.Count == 0) {
				Log.Error("Did not find any placeable spots for mobs");
				return;
			}
			
			PathFinder.SetMapSize(level.Width, level.Height);

			var count = room.Parent.GetPassablePoints(level).Count;
			var weight = (count / 19f + Rnd.Float(0f, 1f)) * room.Parent.GetWeightModifier() * (curseOfBlood ? 2 : 1);

			if (Run.Loop > 0) {
				weight *= Run.Loop * 1.5f + 1f;
			}
			
			while (weight > 0 && (points.Count > 0 || wallPoints.Count > 0)) {
				var id = Rnd.Chances(spawnChances);

				if (id == -1) {
					Log.Error("Failed to generate mobs :O");
					break;
				}
				
				var type = types[id];
				Dot point = null;

				if (type.NearWall) {
					if (wallPoints.Count == 0) {
						continue;
					}

					var index = Rnd.Int(wallPoints.Count);
					point = wallPoints[index];
					wallPoints.RemoveAt(index);
				} else if (type.AwayFromWall) {
					if (wallFreePoints.Count == 0) {
						continue;
					}

					var index = Rnd.Int(wallFreePoints.Count);
					point = wallFreePoints[index];
					wallFreePoints.RemoveAt(index);
				} else {
					if (points.Count == 0) {
						continue;
					}

					var index = Rnd.Int(points.Count);
					point = points[index];
					points.RemoveAt(index);
				}

				if (point == null) {
					continue;
				}

				var mob = (Mob) Activator.CreateInstance(type.Type);
				
				weight -= type.Weight;
				level.Area.Add(mob);
				
				if (type.NearWall) {
					mob.Position = new Vector2(point.X * 16, point.Y * 16 - 8);
				} else {
					mob.BottomCenter = new Vector2(point.X * 16 + 8 + Rnd.Float(-2, 2), point.Y * 16 + 8 + Rnd.Float(-2, 2));
				}
				
				mob.GeneratePrefix();

				if (type.Single) {
					types.RemoveAt(id);
					spawnChances.RemoveAt(id);

					if (types.Count == 0) {
						return;
					}
				}
			}
		}
		
		private void PlaceMobs(Level level, List<Room> rooms) {
			MobRegistry.SetupForBiome(level.Biome.Id);
			// level.CreatePassable(true);
			
			foreach (var room in rooms) {
				if (room.Parent.ShouldSpawnMobs()) {
					PlaceMobs(level, room);
				}
			}	
		}
		
		private void PlaceDoors(RoomDef R) {
			var connected = new Dictionary<RoomDef, DoorPlaceholder>();

			foreach (var pair in R.Connected) {
				connected[pair.Key] = pair.Value;
			}
			
			foreach (var N in connected.Keys) {
				var Door = connected[N];

				if (Door == null) {
					var I = R.Intersect(N);
					var DoorSpots = new List<Dot>();

					foreach (var P in I.GetPoints()) {
						if (R.CanConnect(N, P) && N.CanConnect(R, P)) {
							DoorSpots.Add(P);
						}
					}

					if (DoorSpots.Count > 0) {
						Door = new DoorPlaceholder(DoorSpots[Rnd.Int(DoorSpots.Count)]);
						R.Connected[N] = Door;
						N.Connected[R] = Door;
					} else {
						R.Connected.Remove(N);
						N.Connected.Remove(R);

						throw new Exception($"Failed to connect rooms {R.GetType().Name} and {N.GetType().Name}");
					}
				}
			}
		}

		private void PaintWater(Level Level, List<RoomDef> Rooms) {
			var Lake = Patch.Noise(Water);
			var Ice = Level.Biome is IceBiome;

			foreach (var R in Rooms) {
				if (R is IceConnectionRoom) {
					continue;
				}
				
				var placed = false;
				
				foreach (var P in R.GetWaterPlaceablePoints()) {
					var I = Level.ToIndex((int) P.X, (int) P.Y);
					var T = (Tile) Level.Tiles[I];

					if (Lake[I] && T.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD) && Level.Liquid[I] == 0) {
						Level.Set(I, Ice ? Tile.Ice : Tile.Water);
						placed = true;
					}
				}

				if (!placed) {
					var v = R.GetRandomFreeCell();

					if (v != null) {
						SetBold(Level, v.X, v.Y, Ice ? Tile.Ice : Tile.Water);
					}
				}
			}
		}

		private void PaintCobweb(Level Level, List<RoomDef> Rooms) {
			var Lake = Patch.Noise(Cobweb);

			foreach (var R in Rooms) {
				foreach (var P in R.GetWaterPlaceablePoints()) {
					var I = Level.ToIndex((int) P.X, (int) P.Y);
					var T = (Tile) Level.Tiles[I];
					
					if (Lake[I] && T.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC) && Level.Liquid[I] == 0) {
						Level.Set(I, Tile.Cobweb);
					}
				}
			}
		}

		private void PaintDirt(Level Level, List<RoomDef> Rooms) {
			var Grass = Patch.Noise(Dirt);
			var tile = Level.Biome is DesertBiome ? Tile.Sand : DirtTile;

			foreach (var R in Rooms) {
				foreach (var P in R.GetGrassPlaceablePoints()) {
					if (!Level.IsInside(P.X, P.Y)) {
						continue;
					}
					
					var I = Level.ToIndex((int) P.X, (int) P.Y);

					
					var T = (Tile) Level.Tiles[I];
					
					if (Grass[I] && T.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC) && Level.Liquid[I] == 0) {
						Level.Set(I, tile);
					}
				}
			}
		}

		private void PaintGrass(Level Level, List<RoomDef> Rooms) {
			var Grass = Patch.Noise(this.Grass);
			var Cells = new List<int>();

			foreach (var R in Rooms) {
				foreach (var P in R.GetGrassPlaceablePoints()) {
					var I = Level.ToIndex((int) P.X, (int) P.Y);
					var T = (Tile) Level.Tiles[I];
					
					if (Grass[I] && T.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC) && Level.Liquid[I] == 0) {
						Cells.Add(I);
					}
				}
			}

			foreach (var I in Cells) {
				var Count = 1;

				foreach (var N in PathFinder.Neighbours8) {
					var K = I + N;

					if (Level.IsInside(K) && Grass[K]) {
						Count++;
					}
				}

				Level.Set(I, Tile.Grass);
			}
		}

		public static void PlaceTrees(Level Level, RoomDef Room) {
			for (var Y = Room.Top - 1; Y < Room.Bottom - 1; Y++) {
				for (var X = Room.Left - 1; X < Room.Right - 1; X++) {
					if (Level.Get(X, Y).IsWall() && Level.Get(X, Y - 1).IsWall() &&
					    Level.Get(X - 1, Y).IsWall() && Level.Get(X + 1, Y).IsWall()
					    && !Room.HasDoorsNear(X, Y, 3)
					    && Rnd.Chance(10)) {

						X += 2;
						var plant = new Tree {
							High = true
						};
								
						Level.Area.Add(plant);

						plant.BottomCenter = new Vector2(X * 16 + 8 + Rnd.Float(-4, 4), Y * 16 + 8 + Rnd.Float(-4, 4));
					} else if (Level.Get(X, Y).IsPassable() && Level.Get(X, Y - 1).IsPassable() &&
					           Level.Get(X - 1, Y).IsPassable() && Level.Get(X - 1, Y - 1).IsPassable() &&
					           Level.Get(X + 1, Y).IsPassable() && Level.Get(X + 1, Y - 1).IsPassable() &&
					           Level.Get(X - 1, Y - 2).IsPassable() && Level.Get(X + 1, Y - 2).IsPassable()
					           && !Room.HasDoorsNear(X, Y, 3)
					           && Rnd.Chance(6)) {

						X += 3;
						var plant = new Tree();
						Level.Area.Add(plant);

						plant.BottomCenter = new Vector2(X * 16 + 8 + Rnd.Float(-4, 4), Y * 16 + 8 + Rnd.Float(-4, 4));
					}
				}
			}
		}

		public static void PlacePlants(Level Level, RoomDef Room) {
			for (var Y = Room.Top; Y <= Room.Bottom; Y++) {
				for (int X = Room.Left; X <= Room.Right; X++) {
					if ((Level.Get(X, Y, true).Matches(Tile.Grass, Tile.Dirt) && Rnd.Chance(20)) || (Level.Get(X, Y).Matches(TileFlags.Passable) && Rnd.Chance(5))) {
						var plant = new Plant();
						Level.Area.Add(plant);

						plant.BottomCenter = new Vector2(X * 16 + 8 + Rnd.Float(-4, 4), Y * 16 + 8 + Rnd.Float(-4, 4));
					}
				}
			}
		}

		protected void Decorate(Level Level, List<RoomDef> Rooms) {
			foreach (var Room in Rooms) {
				// Tnt

				if (Level.Biome.HasTnt()) {
					if ((Room is RegularRoom) && Rnd.Chance(20)) {
						for (var i = 0; i < Rnd.Int(1, 4); i++) {
							var p = Room.GetRandomDoorFreeCell();

							if (p != null) {
								var barrel = new ExplodingBarrel();
								Level.Area.Add(barrel);
								barrel.Center = p * 16 + new Vector2(8);
							}
						}
					}
				}

				// Plants
				if (Level.Biome.HasPlants()) {
					PlacePlants(Level, Room);
				}

				if (Events.Halloween) {
					for (var Y = Room.Top; Y <= Room.Bottom; Y++) {
						for (int X = Room.Left; X <= Room.Right; X++) {
							if ((Level.Get(X, Y, true).Matches(Tile.Grass, Tile.Dirt) && Rnd.Chance(20)) || (Level.Get(X, Y).Matches(TileFlags.Passable) && Rnd.Chance(0.5f))) {
								var plant = new Plant();
								Level.Area.Add(plant);
								plant.Variant = 255;
								plant.BottomCenter = new Vector2(X * 16 + 8 + Rnd.Float(-4, 4), Y * 16 + 8 + Rnd.Float(-4, 4));
							}
						}
					}
				}

				if (!(Room is HiveRoom) && Level.Biome.HasTrees()) {
					PlaceTrees(Level, Room);
				}

				// Fireflies
				if (Level.Dark || Rnd.Chance(FirefliesChance)) {
					for (var I = 0; I < (!Level.Dark && Rnd.Chance() ? 1 : Rnd.Int(3, 6)) * Fireflies; I++) {
						Level.Area.Add(new Firefly {
							X = (Room.Left + 2) * 16 + Rnd.Float((Room.GetWidth() - 4) * 16),
							Y = (Room.Top + 2) * 16 + Rnd.Float((Room.GetHeight() - 4) * 16)
						});
					}
				}

				// Cobweb
				if (!(Room is BossRoom) && Level.Biome.HasCobwebs()) {
					for (var Y = Room.Top; Y <= Room.Bottom; Y++) {
						for (int X = Room.Left; X <= Room.Right; X++) {
							if (Level.Get(X, Y).IsSimpleWall()) {
								if (Y > Room.Top && X > Room.Left && Level.Get(X - 1, Y - 1).IsSimpleWall() && !Level.Get(X, Y - 1).IsSimpleWall() && Rnd.Chance(20)) {
									Level.Area.Add(new SlicedProp("cobweb_c", Layers.WallDecor) {
										X = X * 16,
										Y = Y * 16 - 24
									});
								} else if (Y > Room.Top && X < Room.Right && Level.Get(X + 1, Y - 1).IsSimpleWall() && !Level.Get(X, Y - 1).IsSimpleWall() && Rnd.Chance(20)) {
									Level.Area.Add(new SlicedProp("cobweb_d", Layers.WallDecor) {
										X = X * 16,
										Y = Y * 16 - 24
									});
								} else if (Y < Room.Bottom - 1 && X > Room.Left && Level.Get(X - 1, Y + 1).IsSimpleWall() && !Level.Get(X, Y + 1).IsSimpleWall() && Rnd.Chance(20)) {
									Level.Area.Add(new SlicedProp("cobweb_a", Layers.WallDecor) {
										X = X * 16,
										Y = Y * 16 + 8
									});
								} else if (Y < Room.Bottom - 1 && X < Room.Right && Level.Get(X + 1, Y + 1).IsSimpleWall() && !Level.Get(X, Y + 1).IsSimpleWall() && Rnd.Chance(20)) {
									Level.Area.Add(new SlicedProp("cobweb_b", Layers.WallDecor) {
										X = X * 16,
										Y = Y * 16 + 8
									});
								}
							}
						}
					}
				}

				
				if (!(Room is SecretRoom || Room is TreasureRoom || Room is RegularRoom || Room is EntranceRoom || Room is ConnectionRoom) || Run.Depth < 1) {
					continue;
				}

				// Paintings && Torches
				var ht = Level.Biome.HasTorches();
				var hp = Level.Biome.HasPaintings();
				
				if (ht || hp) {
					for (int X = Room.Left + 1; X < Room.Right; X++) {
						var s = Room is SecretRoom;
						var t = Level.Get(X, Room.Top);

						if (t != Tile.Crack && t.IsWall() && !Level.Get(X, Room.Top + 1).IsWall() && Rnd.Chance(s ? 50 : 30)) {
							if (!s && Rnd.Chance()) {
								if (ht) {
									var torch = new WallTorch();
									Level.Area.Add(torch);
									torch.CenterX = X * 16 + 8 + Rnd.Float(-1, 1);
									torch.CenterY = Room.Top * 16 + 13;
								}
							} else if (hp) {
								var painting = PaintingRegistry.Generate(Level.Biome);
								Level.Area.Add(painting);

								painting.CenterX = X * 16 + 8 + Rnd.Float(-1, 1);
								painting.Bottom = Room.Top * 16 + 17;
							}
						}
					}
				}

				if (!Level.Biome.HasBrekables() || Room is SecretRoom || Room is TreasureRoom || Room is ConnectionRoom || Room is EntranceRoom) {
					continue;
				}

				var types = new List<string>();
				var infos = Level.Biome is IceBiome ? BreakableProp.IceInfos : BreakableProp.Infos;

				for (var i = 0; i < Rnd.Int(2, 3); i++) {
					types.Add(infos[Rnd.Int(infos.Length)]);
				}
				
				for (int i = 0; i < Rnd.IntCentred(2, 7); i++) {
					var prop = new BreakableProp {
						Sprite = types[Rnd.Int(types.Count)]
					};
					
					var point = Room.GetRandomDoorFreeCell();

					if (point == null) {
						continue;
					}
					
					Level.Area.Add(prop);
					prop.Center = new Vector2(point.X * 16 + 8 + Rnd.Float(-3, 3), point.Y * 16 + 8 + Rnd.Float(-3, 3));
				}
			}
		}

		public static void PaintDoor(Level Level, RoomDef R) {
			foreach (var N in R.Connected.Keys) {
				var D = R.Connected[N];
				PlaceDoor(Level, R, D, N);
			}
		}

		public static void PlaceDoor(Level Level, RoomDef R, DoorPlaceholder D, RoomDef from) {
			var T = Level.Get(D.X, D.Y);
			var type = D.Type;

			var gt = type != DoorPlaceholder.Variant.Empty && type != DoorPlaceholder.Variant.Maze && type != DoorPlaceholder.Variant.Secret;
			var vertical = Level.Get(D.X, D.Y + 1).IsWall() && Level.Get(D.X, D.Y - 1).IsWall();
			
			if (gt && !T.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD, Tile.Crack)) {
				Door door = null;

				switch (type) {
					case DoorPlaceholder.Variant.Locked: 
						door = new SpecialDoor();
						break;
					
					case DoorPlaceholder.Variant.Red: 
						door = new RedDoor();
						break;
					
					case DoorPlaceholder.Variant.Boss: 
						door = new BossDoor();
						break;
					
					case DoorPlaceholder.Variant.Treasure:
						door = new TreasureDoor();
						break;
					
					case DoorPlaceholder.Variant.Scourged:
						door = new ScourgedDoor();
						break;
					
					case DoorPlaceholder.Variant.Payed:
						door = new PayedDoor();
						break;
					
					case DoorPlaceholder.Variant.Head:
						// door = new HeadDoor();
						return;
					
					case DoorPlaceholder.Variant.Spiked:
						door = new SpikedDoor();
						break;
					
					case DoorPlaceholder.Variant.Challenge:
						door = new ChallengeDoor();
						break;
					
					case DoorPlaceholder.Variant.Shop:
						door = new ShopDoor();
						break;
				
					default: 
						door = new LockableDoor();
						break;
				}

				door.Vertical = vertical;
				Level.Area.Add(door);

				var offset = door.GetOffset();

				door.CenterX = D.X * 16 + 8 + offset.X;
				door.Bottom = D.Y * 16 + 17.01f + offset.Y - (door is CustomDoor ? (door.Vertical ? 0 : 8) : 0); // .1f so that it's depth sorted to the front of the wall

				if (!(door is HeadDoor)) {
					if (door.Vertical) {
						if (type != DoorPlaceholder.Variant.Hidden) {
							if (!Level.Get(D.X + 1, D.Y).Matches(TileFlags.Passable)) {
								Level.Set(D.X + 1, D.Y, Tiles.RandomFloor());
							}

							if (!Level.Get(D.X - 1, D.Y).Matches(TileFlags.Passable)) {
								Level.Set(D.X - 1, D.Y, Tiles.RandomFloor());
							}
						}
					} else {
						if (type != DoorPlaceholder.Variant.Hidden) {
							if (!Level.Get(D.X, D.Y + 1).Matches(TileFlags.Passable)) {
								Level.Set(D.X, D.Y + 1, Tiles.RandomFloor());
							}

							if (!Level.Get(D.X, D.Y - 1).Matches(TileFlags.Passable)) {
								Level.Set(D.X, D.Y - 1, Tiles.RandomFloor());
							}
						}
					}

					Level.Set(D.X, D.Y, Tiles.RandomFloor());
				}
			} else if (type == DoorPlaceholder.Variant.Hidden) {
				Level.Set(D.X, D.Y, Level.Biome is IceBiome ? Tile.WallB : Tile.WallA);
			} else if (type == DoorPlaceholder.Variant.Secret) {
				Level.Set(D.X, D.Y, Tile.Crack);
				
				if (vertical) {
					if (Level.Get(D.X + 1, D.Y).Matches(Tile.WallA, Tile.WallB, Tile.Planks, Tile.Rock, Tile.TintedRock, Tile.MetalBlock)) {
						Level.Set(D.X + 1, D.Y, Tiles.RandomFloor());
					}

					if (!Level.Get(D.X - 1, D.Y).Matches(Tile.WallA, Tile.WallB, Tile.Planks, Tile.Rock, Tile.TintedRock, Tile.MetalBlock)) {
						Level.Set(D.X - 1, D.Y, Tiles.RandomFloor());
					}
				} else {
					if (!Level.Get(D.X, D.Y + 1).Matches(Tile.WallA, Tile.WallB, Tile.Planks, Tile.Rock, Tile.TintedRock, Tile.MetalBlock)) {
						Level.Set(D.X, D.Y + 1, Tiles.RandomFloor());
					}

					if (!Level.Get(D.X, D.Y - 1).Matches(Tile.WallA, Tile.WallB, Tile.Planks, Tile.Rock, Tile.TintedRock, Tile.MetalBlock)) {
						Level.Set(D.X, D.Y - 1, Tiles.RandomFloor());
					}
				}
			} else if (type == DoorPlaceholder.Variant.Empty) { 
				Level.Set(D.X, D.Y, Tiles.RandomFloor());
			}
		}

		private void PaintDoors(Level Level, List<RoomDef> Rooms) {
			foreach (var R in Rooms) {
				PaintDoor(Level, R);
			}
		}

		public static void Set(Level Level, int cell, Tile Value) {
			if (Clip != null && !Clip.Contains(Level.FromIndexX(cell), Level.FromIndexY(cell))) {
				return;
			}

			Level.Set(cell, Value);
		}

		public static void Set(Level Level, int X, int Y, Tile Value, bool bold = false, bool walls = false) {
			if (bold) {
				SetBold(Level, X, Y, Value, walls);
				return;
			}
			
			Set(Level, Level.ToIndex(X, Y), Value);
		}

		public static void SetBold(Level Level, int X, int Y, Tile Value, bool walls = false) {
			for (var Yy = Y - 1; Yy < Y + 2; Yy++) {
				for (var Xx = X - 1; Xx < X + 2; Xx++) {
					if (!Level.IsInside(Xx, Yy)) {
						continue;
					}
					
					if (Xx != X || Yy != Y) {
						if (!walls && Level.Get(Xx, Yy).IsWall()) {
							continue;
						}
					}

					Set(Level, Xx, Yy, Value);
				}
			}
		}

		public static void Set(Level Level, Dot P, Tile Value) {
			Set(Level, (int) P.X, (int) P.Y, Value);
		}
		
		public static void Call(Level Level, int X, int Y, int W, int H, Action<int, int> callback) {
			for (var Yy = Y; Yy < Y + H; Yy++) {
				for (var Xx = X; Xx < X + W; Xx++) {
					callback(Xx, Yy);
				}
			}
		}

		public static void Call(Level level, Rect rect, int m, Action<int, int> callback) {
			rect = rect.Shrink(m);
			Call(level, rect.Left, rect.Top, rect.GetWidth(), rect.GetHeight(), callback);
		}

		public static void Fill(Level Level, int X, int Y, int W, int H, Tile Value) {
			for (var Yy = Y; Yy < Y + H; Yy++) {
				for (var Xx = X; Xx < X + W; Xx++) {
					Set(Level, Xx, Yy, Value);
				}
			}
		}

		public static void Rect(Level level, Rect rect, int m, Tile value, bool bold = false) {
			rect = rect.Shrink(m);
			Rect(level, rect.Left, rect.Top, rect.GetWidth(), rect.GetHeight(), value, bold);
		}

		public static void Rect(Level level, int X, int Y, int W, int H, Tile value, bool bold = false) {
			DrawLine(level, new Dot(X, Y), new Dot(X + W, Y), value, bold);
			DrawLine(level, new Dot(X, Y + H), new Dot(X + W, Y + H), value, bold);
			DrawLine(level, new Dot(X, Y), new Dot(X, Y + H), value, bold);
			DrawLine(level, new Dot(X + W, Y), new Dot(X + W, Y + H), value, bold);
		}

		public static void Triangle(Level Level, Dot From, Dot P1, Dot P2, Tile V) {
			if ((int) P1.X != (int) P2.X) {
				for (var X = P1.X; X < P2.X; X++) {
					DrawLine(Level, From, new Dot(X, P1.Y), V);
				}
			} else {
				for (var Y = P1.Y; Y < P2.Y; Y++) {
					DrawLine(Level, From, new Dot(P1.X, Y), V);
				}
			}
		}

		public static void Fill(Level Level, Rect Rect, Tile Value) {
			Fill(Level, Rect.Left, Rect.Top, Rect.GetWidth(), Rect.GetHeight(), Value);
		}

		public static void Fill(Level Level, Rect Rect, int M, Tile Value) {
			Fill(Level, Rect.Left + M, Rect.Top + M, Rect.GetWidth() - M * 2, Rect.GetHeight() - M * 2, Value);
		}

		public static void Fill(Level Level, Rect Rect, int L, int T, int R, int B, Tile Value) {
			Fill(Level, Rect.Left + L, Rect.Top + T, Rect.GetWidth() - (L + R), Rect.GetHeight() - (T + B), Value);
		}

		public static void DrawLine(Level Level, Dot From, Dot To, Tile Value, bool Bold = false) {
			float X = From.X;
			float Y = From.Y;
			float Dx = To.X - From.X;
			float Dy = To.Y - From.Y;
			var MovingbyX = Math.Abs(Dx) >= Math.Abs(Dy);

			if (MovingbyX) {
				Dy /= Math.Abs(Dx);
				Dx /= Math.Abs(Dx);
			} else {
				Dx /= Math.Abs(Dy);
				Dy /= Math.Abs(Dy);
			}

			if (Bold) {
				SetBold(Level, (int) Math.Round(X), (int) Math.Round(Y), Value);
			} else {
				Set(Level, (int) Math.Round(X), (int) Math.Round(Y), Value);
			}

			while (MovingbyX && (int) To.X != (int) X || !MovingbyX && (int) To.Y != (int) Y) {
				X += Dx;
				Y += Dy;

				if (Bold) {
					SetBold(Level, (int) Math.Round(X), (int) Math.Round(Y), Value);
				} else {
					Set(Level, (int) Math.Round(X), (int) Math.Round(Y), Value);
				}
			}
		}

		public static void FillEllipse(Level Level, Rect Rect, Tile Value) {
			FillEllipse(Level, Rect.Left, Rect.Top, Rect.GetWidth(), Rect.GetHeight(), Value);
		}

		public static void FillEllipse(Level Level, Rect Rect, int M, Tile Value) {
			Rect = Rect.Shrink(M);
			FillEllipse(Level, Rect.Left, Rect.Top, Rect.GetWidth(), Rect.GetHeight(), Value);
		}

		public static void FillEllipse(Level Level, int X, int Y, int W, int H, Tile Value) {
			double RadH = H / 2f;
			double RadW = W / 2f;

			for (var I = 0; I < H; I++) {
				var RowY = -RadH + 0.5 + I;
				var RowW = 2.0 * Math.Sqrt(RadW * RadW * (1.0 - RowY * RowY / (RadH * RadH)));

				if (W % 2 == 0) {
					RowW = Math.Round(RowW / 2.0) * 2.0;
				} else {
					RowW = Math.Floor(RowW / 2.0) * 2.0;
					RowW++;
				}

				var Cell = X + (W - (int) RowW) / 2 + (Y + I) * Level.Width;

				for (var J = Cell; J < Cell + RowW; J++) {
					Level.Set(J, Value);
				}
			}
		}

		public static void Ellipse(Level Level, Rect Rect, int m, Tile Value, bool bold = false) {
			Rect = Rect.Shrink(m);
			Ellipse(Level, Rect.Left, Rect.Top, Rect.GetWidth(), Rect.GetHeight(), Value, bold);
		}
		
		public static void Ellipse(Level Level, Rect Rect, Tile Value, bool bold = false) {
			Ellipse(Level, Rect.Left, Rect.Top, Rect.GetWidth(), Rect.GetHeight(), Value, bold);
		}
		
		// To be tested
		public static void Ellipse(Level Level, int X, int Y, int W, int H, Tile Value, bool bold) {
			double RadH = H / 2f;
			double RadW = W / 2f;

			for (var I = 0; I < H; I++) {
				var RowY = -RadH + 0.5 + I;
				var RowW = 2.0 * Math.Sqrt(RadW * RadW * (1.0 - RowY * RowY / (RadH * RadH)));

				if (W % 2 == 0) {
					RowW = Math.Round(RowW / 2.0) * 2.0;
				} else {
					RowW = Math.Floor(RowW / 2.0) * 2.0;
					RowW++;
				}

				var Cell = X + (W - (int) RowW) / 2 + (Y + I) * Level.Width;
				var CellB = (int) (Cell + RowW - 1);

				if (I == 0 || I == H - 1) {
					for (var J = Cell - 1; J <= Cell + RowW; J++) {
						if (bold) {
							SetBold(Level, Level.FromIndexX(J), Level.FromIndexY(J), Value);
						} else {
							Level.Set(J, Value);
						}
					}
				} else {
					if (bold) {
						SetBold(Level, Level.FromIndexX(Cell), Level.FromIndexY(Cell), Value);
						SetBold(Level, Level.FromIndexX(CellB), Level.FromIndexY(CellB), Value);	
					} else {
						Level.Set(Cell, Value);
						Level.Set(CellB, Value);	
					}
				}
			}
		}

		public static void Prefab(Level level, string id, int x, int y) {
			var prefab = Prefabs.Get(id);

			if (prefab == null) {
				Log.Error($"Unknown prefab {id}");
				return;
			}
			
			prefab.Place(level, x, y);
		}
	}
}