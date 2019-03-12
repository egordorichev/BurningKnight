using System;
using System.Collections.Generic;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.rooms;
using BurningKnight.state;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.level.painters {
	public class Painter {
		private float Cobweb = 0.1f;
		private float Dirt = 0.5f;
		private float Grass = 0.5f;
		private float Water = 0.5f;

		public Painter SetWater(float V) {
			Water = V;

			return this;
		}

		public Painter SetCobweb(float V) {
			Cobweb = V;

			return this;
		}

		public Painter SetDirt(float V) {
			Dirt = V;

			return this;
		}

		public Painter SetGrass(float V) {
			Grass = V;

			return this;
		}

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
			var Sz = 0;
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
			Log.Info("Setting level size to " + (1 + RightMost) + ":" + (BottomMost + 1));
			Level.Width = RightMost + 1;
			Level.Height = BottomMost + 1;
			
			Level.Setup();

			for (int i = 0; i < Level.Size; i++) {
				Level.Tiles[i] = (byte) Tile.WallA;
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

				/*if (Level is IceLevel)
					for (var Y = Room.Top; Y <= Room.Bottom; Y++)
					for (var X = Room.Left; X <= Room.Right; X++) {
						int I = Level.ToIndex(X, Y);

						if (Level.LiquidData[I] == Terrain.WATER) {
							Level.LiquidData[I] = 0;
							Level.Set(I, Terrain.ICE);
						}
					}*/

				/*if (!(Room is BossEntranceRoom) && Level is ForestLevel && Random.Chance(70))
					for (var I = 0; I < Random.NewInt(1, 4); I++) {
						var Point = Room.GetRandomFreeCell();

						if (Point != null) {
							var Bush = new Bush();
							Bush.X = Point.X * 16 + Random.NewFloat(-4, 4);
							Bush.Y = Point.Y * 16 + Random.NewFloat(-4, 4);
							Dungeon.Area.Add(Bush.Add());
						}
					}*/
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

			Decorate(Level, Rooms);
			PaintDoors(Level, Rooms);
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
			var Lake = Patch.Generate(Water, 5);
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
			var Lake = Patch.Generate(Water, 5);

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
			var Grass = Patch.Generate(Dirt, 5);

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
			var Grass = Patch.Generate(this.Grass, 5);
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
				/*
				if (Random.Chance(60)) {
					for (var I = 0; I < (Random.Chance(50) ? 1 : Random.Int(3, 6)); I++) {
						var Fly = new Firefly();
						Fly.X = (Room.Left + 2) * 16 + Random.Float((Room.GetWidth() - 4) * 16);
						Fly.Y = (Room.Top + 2) * 16 + Random.Float((Room.GetHeight() - 4) * 16);
						Dungeon.Area.Add(Fly.Add());
					}
				}*/

				/*for (var Y = Room.Top; Y <= Room.Bottom; Y++) {
					
					if (Dungeon.Depth > -1 && Level.Get(X, Y) == Terrain.WALL && !(Level is IceLevel || Level is TechLevel)) {
						if (Y > Room.Top && X > Room.Left && Level.Get(X - 1, Y - 1) == Terrain.WALL && Level.Get(X, Y - 1) != Terrain.WALL && Random.Chance(20)) {
							var Web = new Cobweb();
							Web.X = X * 16;
							Web.Y = Y * 16 - 16;
							Web.Side = 0;
							Dungeon.Area.Add(Web);
							LevelSave.Add(Web);
						} else if (Y > Room.Top && X < Room.Right && Level.Get(X + 1, Y - 1) == Terrain.WALL && Level.Get(X, Y - 1) != Terrain.WALL && Random.Chance(20)) {
							var Web = new Cobweb();
							Web.X = X * 16;
							Web.Y = Y * 16 - 16;
							Web.Side = 1;
							Dungeon.Area.Add(Web);
							LevelSave.Add(Web);
						} else if (Y < Room.Bottom - 1 && X > Room.Left && Level.Get(X - 1, Y + 1) == Terrain.WALL && Level.Get(X, Y + 1) != Terrain.WALL && Random.Chance(20)) {
							var Web = new Cobweb();
							Web.X = X * 16;
							Web.Y = Y * 16 + 16;
							Web.Side = 2;
							Dungeon.Area.Add(Web);
							LevelSave.Add(Web);
						} else if (Y < Room.Bottom - 1 && X < Room.Right && Level.Get(X + 1, Y + 1) == Terrain.WALL && Level.Get(X, Y + 1) != Terrain.WALL && Random.Chance(20)) {
							var Web = new Cobweb();
							Web.X = X * 16;
							Web.Y = Y * 16 + 16;
							Web.Side = 3;
							Dungeon.Area.Add(Web);
							LevelSave.Add(Web);
						}
					}
				}*/
			}
		}

		private void PaintDoors(Level Level, List<RoomDef> Rooms) {
			foreach (var R in Rooms) {
				foreach (var N in R.Connected.Keys) {
					var D = R.Connected[N];
					var T = Level.Get(D.X, D.Y);
					var type = D.Type;

					var Gt = type != DoorPlaceholder.Variant.Empty && type != DoorPlaceholder.Variant.Maze &&
					         type != DoorPlaceholder.Variant.Tunnel && type != DoorPlaceholder.Variant.Secret;

					if (Gt && !T.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD, Tile.Crack)) {
						/*var Door = new Door((int) D.X, (int) D.Y, !Level.CheckFor(D.X + 1, D.Y, TileFlags.Solid));

						if (type == DoorPlaceholder.Variant.Regular) {
							D.Type = type = DoorPlaceholder.Variant.Enemy;
						}

						//Door.AutoLock = type == DoorPlaceholder.Variant.Enemy || type == DoorPlaceholder.Variant.Boss;
						//Door.Lock = type == DoorPlaceholder.Variant.Locked;

						if (type == DoorPlaceholder.Variant.Locked) {
							// Door.Key = KeyC.GetType();
						} else if (type == DoorPlaceholder.Variant.Boss) {
						//	Door.BkDoor = true;
						}

						//Door.Lockable = Door.Lock;
						Level.Area.Add(Door);
					}

					if (type == DoorPlaceholder.Variant.Secret) {
						Level.Set(D.X, D.Y, Tile.Crack);
					} else {
						var F = Tiles.RandomFloor();

						for (var Yy = -1; Yy <= 1; Yy++) {
							for (var Xx = -1; Xx <= 1; Xx++) {
								if (Math.Abs(Xx) + Math.Abs(Yy) == 1) {
									var Tl = Level.Get(D.X + Xx, D.Y + Yy);

									if (!Tl.Matches(Tile.Wall, Tile.Crack, Tile.Chasm)) {
										F = Tl;

										break;
									}
								}
							}
						}*/

						Level.Set(D.X, D.Y, /*F*/ Tiles.RandomFloor());
					}
				}
			}
		}

		public static void Set(Level Level, int cell, Tile Value) {
			Level.Set(cell, Value);
		}

		public static void Set(Level Level, int X, int Y, Tile Value) {
			Set(Level, Level.ToIndex(X, Y), Value);
		}

		public static void SetBold(Level Level, int X, int Y, Tile Value) {
			for (var Yy = Y - 1; Yy < Y + 2; Yy++) {
				for (var Xx = X - 1; Xx < X + 2; Xx++) {
					var T = Level.Get(X, Y);

					if (T != Value) {
						if (Xx != X || Yy != Y) {
							if (T == Tile.WallA || T == Tile.WallB) {
								continue;
							}
						}

						Set(Level, Xx, Yy, Value);
					}
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
			FillEllipse(Level, Rect.Left + M, Rect.Top + M, Rect.GetWidth() - M * 2, Rect.GetHeight() - M * 2, Value);
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
	}
}