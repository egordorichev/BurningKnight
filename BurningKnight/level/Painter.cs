using System;
using System.Collections.Generic;
using BurningKnight.assets.prefabs;
using BurningKnight.entity;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.door;
using BurningKnight.entity.fx;
using BurningKnight.level.entities;
using BurningKnight.level.paintings;
using BurningKnight.level.rooms;
using BurningKnight.level.rooms.connection;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.rooms.secret;
using BurningKnight.level.rooms.treasure;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.level {
	public class Painter {
		public float Cobweb = 0.1f;
		public float Dirt = 0.4f;
		public float Grass = 0.4f;
		public float Water = 0.4f;

		public void Paint(Level Level, List<RoomDef> Rooms) {
			if (Rooms == null) {
				return;
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

			for (int i = Rooms.Count - 1; i >= 0; i--) {
				var Room = Rooms[i];
				PlaceDoors(Room);
				Room.PaintFloor(Level);
				Room.Paint(Level);

				if (Run.Depth == 1) {
					for (var Y = Room.Top; Y <= Room.Bottom; Y++) {
						for (var X = Room.Left; X <= Room.Right; X++) {
							var I = Level.ToIndex(X, Y);

							if (Level.Liquid[I] == (int) Tile.Lava) {
								Level.Liquid[I] = 0;
								Level.Set(I, Tile.Chasm);
							}
						}
					}
				}
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
			
			PlaceMobs(Level, Rooms);
		}

		public static void PlaceMobs(Level level, RoomDef room) {
			var mobs = new List<MobInfo>(MobRegistry.Current);
			room.ModifyMobList(mobs);
			var chances = new float[mobs.Count];

			for (int i = 0; i < mobs.Count; i++) {
				chances[i] = room.WeightMob(mobs[i], mobs[i].GetChanceFor(level.Biome.Id));
			}

			var types = new List<Type>();
			var spawnChances = new List<float>();

			for (int i = 0; i < Random.Int(2, 6); i++) {
				var type = mobs[Random.Chances(chances)].Type;
				var found = false;
				
				foreach (var t in types) {
					if (t == type) {
						found = true;
						break;
					}
				}

				if (found) {
					i--;
				} else {
					types.Add(type);
					spawnChances.Add(((Mob) Activator.CreateInstance(type)).GetSpawnChance());
				}
			}

			if (types.Count == 0) {
				Log.Warning($"No mobs detected to spawn in {level.Biome.Id} biome");
				return;
			}

			var weight = room.GetWidth() * room.GetHeight() / 20f + Random.Float(0f, 3f);

			while (weight > 0) {
				var id = Random.Chances(spawnChances);
				var type = types[id];
				var mob = (Mob) Activator.CreateInstance(type);
				var wall = mob.SpawnsNearWall();
				
				var point = wall ? room.GetRandomCellNearWall() : room.GetRandomDoorFreeCell();

				if (!point.HasValue) {
					continue;
				}
				
				weight -= mob.GetWeight();

				if (wall) {
					mob.Position = new Vector2(point.Value.X * 16, point.Value.Y * 16 - 8);
				} else {
					mob.Center = new Vector2(point.Value.X * 16 + 8 + Random.Float(-2, 2), point.Value.Y * 16 + 8 + Random.Float(-2, 2));
				}

				level.Area.Add(mob);

				if (!mob.CanSpawnMultiple()) {
					types.RemoveAt(id);
					spawnChances.RemoveAt(id);

					if (types.Count == 0) {
						return;
					}
				}
			}
		}
		
		private void PlaceMobs(Level level, List<RoomDef> rooms) {
			MobRegistry.SetupForBiome(level.Biome.Id);
			
			foreach (var room in rooms) {
				if (room.ShouldSpawnMobs()) {
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
					var DoorSpots = new List<Vector2>();

					foreach (var P in I.GetPoints()) {
						if (R.CanConnect(P) && N.CanConnect(P)) {
							DoorSpots.Add(P);
						}
					}

					if (DoorSpots.Count > 0) {
						var Point = DoorSpots[Random.Int(DoorSpots.Count)];
						Door = new DoorPlaceholder(Point);
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
			var Ice = false; // Level is IceLevel;

			foreach (var R in Rooms) {
				foreach (var P in R.WaterPlaceablePoints()) {
					var I = Level.ToIndex((int) P.X, (int) P.Y);
					var T = (Tile) Level.Tiles[I];

					if (Lake[I] && T.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC) && Level.Liquid[I] == 0) {
						Level.Set(I, Ice ? Tile.Ice : Tile.Water);
					}
				}
			}
		}

		private void PaintCobweb(Level Level, List<RoomDef> Rooms) {
			var Lake = Patch.Noise(Cobweb);

			foreach (var R in Rooms) {
				foreach (var P in R.WaterPlaceablePoints()) {
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

			foreach (var R in Rooms) {
				foreach (var P in R.GrassPlaceablePoints()) {
					var I = Level.ToIndex((int) P.X, (int) P.Y);
					var T = (Tile) Level.Tiles[I];
					
					if (Grass[I] && T.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC) && Level.Liquid[I] == 0) {
						Level.Set(I, Tile.Dirt);
					}
				}
			}
		}

		private void PaintGrass(Level Level, List<RoomDef> Rooms) {
			var Grass = Patch.Noise(this.Grass);
			var Cells = new List<int>();

			foreach (var R in Rooms) {
				foreach (var P in R.GrassPlaceablePoints()) {
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

				var High = Random.Float() < Count / 12f;
				Level.Set(I, High ? Tile.HighGrass : Tile.Grass);
			}
		}

		protected void Decorate(Level Level, List<RoomDef> Rooms) {
			foreach (var Room in Rooms) {
				if (Random.Chance(60)) {
					for (var I = 0; I < (Random.Chance(50) ? 1 : Random.Int(3, 6)); I++) {
						Level.Area.Add(new Firefly {
							X = (Room.Left + 2) * 16 + Random.Float((Room.GetWidth() - 4) * 16),
							Y = (Room.Top + 2) * 16 + Random.Float((Room.GetHeight() - 4) * 16)
						});
					}
				}

				for (var Y = Room.Top; Y <= Room.Bottom; Y++) {
					for (int X = Room.Left; X <= Room.Right; X++) {
						if (Level.Get(X, Y).IsWall()) {
							if (Y > Room.Top && X > Room.Left && Level.Get(X - 1, Y - 1).IsWall() && !Level.Get(X, Y - 1).IsWall() && Random.Chance(20)) {
								Level.Area.Add(new Prop("cobweb_c", Layers.WallDecor) {
									X = X * 16,
									Y = Y * 16 - 24
								});
							} else if (Y > Room.Top && X < Room.Right && Level.Get(X + 1, Y - 1).IsWall() && !Level.Get(X, Y - 1).IsWall() && Random.Chance(20)) {
								Level.Area.Add(new Prop("cobweb_d", Layers.WallDecor) {
									X = X * 16,
									Y = Y * 16 - 24
								});
							} else if (Y < Room.Bottom - 1 && X > Room.Left && Level.Get(X - 1, Y + 1).IsWall() && !Level.Get(X, Y + 1).IsWall() && Random.Chance(20)) {
								Level.Area.Add(new Prop("cobweb_a", Layers.WallDecor) {
									X = X * 16,
									Y = Y * 16 + 8
								});
							} else if (Y < Room.Bottom - 1 && X < Room.Right && Level.Get(X + 1, Y + 1).IsWall() && !Level.Get(X, Y + 1).IsWall() && Random.Chance(20)) {
								Level.Area.Add(new Prop("cobweb_b", Layers.WallDecor) {
									X = X * 16,
									Y = Y * 16 + 8
								});
							}
						}
					}
				}

				if (!(Room is SecretRoom || Room is TreasureRoom || Room is RegularRoom || Room is EntranceRoom || Room is ExitRoom || Room is ConnectionRoom) || Run.Depth < 1) {
					continue;
				}

				for (int X = Room.Left + 1; X < Room.Right; X++) {
					if (Level.Get(X, Room.Top).IsWall() && Random.Chance(20)) {
						var painting = PaintingRegistry.Generate(Level.Biome);
						Level.Area.Add(painting);

						painting.CenterX = X * 16 + 8 + Random.Float(-1, 1);
						painting.CenterY = Room.Top * 16 + 13;
					}
				}

				if (Room is SecretRoom || Room is TreasureRoom || Room is ConnectionRoom) {
					continue;
				}
				
				var types = new List<string>();

				for (var i = 0; i < Random.Int(2, 3); i++) {
					types.Add(BreakableProp.Infos[Random.Int(BreakableProp.Infos.Length)]);
				}
				
				for (int i = 0; i < Random.IntCentred(2, 7); i++) {
					var prop = new BreakableProp {
						Sprite = types[Random.Int(types.Count)]
					};
					
					var point = Room.GetRandomDoorFreeCell();

					if (!point.HasValue) {
						continue;
					}
					
					Level.Area.Add(prop);
					prop.Center = new Vector2(point.Value.X * 16 + 8 + Random.Float(-3, 3), point.Value.Y * 16 + 8 + Random.Float(-3, 3));
				}
			}
		}

		private void PaintDoors(Level Level, List<RoomDef> Rooms) {
			foreach (var R in Rooms) {
				foreach (var N in R.Connected.Keys) {
					var D = R.Connected[N];
					var T = Level.Get(D.X, D.Y);
					var type = D.Type;

					var gt = type != DoorPlaceholder.Variant.Empty && type != DoorPlaceholder.Variant.Maze &&
					         type != DoorPlaceholder.Variant.Tunnel && type != DoorPlaceholder.Variant.Secret;

					if (gt && !T.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD, Tile.Crack)) {
						var door = new LockableDoor();

						door.X = D.X * 16;
						door.Y = D.Y * 16;
						var tile = Level.Get(D.X, D.Y + 1);
						door.FacingSide = tile.IsWall() && tile != Tile.Planks;

						if (door.FacingSide) {
							door.Y -= 8;
							door.X += 6;
						} else {
							door.Y -= 2;
						}
						
						Level.Area.Add(door);

						Level.Set(D.X, D.Y, Tiles.RandomFloor());
					} else if (type == DoorPlaceholder.Variant.Secret) {
						Level.Set(D.X, D.Y, Tile.Crack);
					} else if (type == DoorPlaceholder.Variant.Tunnel) {
						Level.Set(D.X, D.Y, Tiles.RandomFloor());
					}
				}
			}
		}

		public static void Set(Level Level, int cell, Tile Value) {
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

		public static void Set(Level Level, Vector2 P, Tile Value) {
			Set(Level, (int) P.X, (int) P.Y, Value);
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
			Rect(level, rect.Left, rect.Top, rect.GetWidth() - 1, rect.GetHeight() - 1, value, bold);
		}

		public static void Rect(Level level, int X, int Y, int W, int H, Tile value, bool bold = false) {
			DrawLine(level, new Vector2(X, Y), new Vector2(X + W, Y), value, bold);
			DrawLine(level, new Vector2(X, Y + H), new Vector2(X + W, Y + H), value, bold);
			DrawLine(level, new Vector2(X, Y), new Vector2(X, Y + H), value, bold);
			DrawLine(level, new Vector2(X + W, Y), new Vector2(X + W, Y + H), value, bold);
		}

		public static void Triangle(Level Level, Vector2 From, Vector2 P1, Vector2 P2, Tile V) {
			if ((int) P1.X != (int) P2.X) {
				for (var X = P1.X; X < P2.X; X++) {
					DrawLine(Level, From, new Vector2(X, P1.Y), V);
				}
			} else {
				for (var Y = P1.Y; Y < P2.Y; Y++) {
					DrawLine(Level, From, new Vector2(P1.X, Y), V);
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

		public static void DrawLine(Level Level, Vector2 From, Vector2 To, Tile Value, bool Bold = false) {
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