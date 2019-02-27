using System;
using System.Collections.Generic;
using BurningKnight.entity.level.entities.decor;
using BurningKnight.entity.level.rooms;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;
using Door = BurningKnight.entity.level.entities.Door;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.level.painters {
	public class Painter {
		private float Cobweb;
		private float Dirt;
		private float Grass;
		private float Water;

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

		public void Paint(Level Level, List<Room> Rooms) {
			if (Rooms == null) {
				return;
			}

			int LeftMost = Int32.MaxValue;
			int TopMost = Int32.MaxValue;

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
			Level.SetSize(RightMost + 1, BottomMost + 1);
			Level.GenerateDecor();
			Level.Fill();

			foreach (var Room in Rooms) {
				PlaceDoors(Room);
				Room.Paint(Level);

				if (Run.Depth == 1) { // or ice
					for (var Y = Room.Top; Y <= Room.Bottom; Y++)
					for (var X = Room.Left; X <= Room.Right; X++) {
						int I = Level.ToIndex(X, Y);

						if (Level.LiquidData[I] == Terrain.LAVA) {
							Level.LiquidData[I] = 0;
							Level.Set(I, Terrain.CHASM);
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


				if (Room.Hidden) {
					for (var Y = Room.Top; Y <= Room.Bottom; Y++) {
						for (var X = Room.Left; X <= Room.Right; X++) {
							Level.Hide(X, Y);
						}
					}
				}
			}

			if (PathFinder.NEIGHBOURS8 == null) {
				PathFinder.SetMapSize(Level.GetWidth(), Level.GetHeight());
			}

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

		private void PaintWater(Level Level, List<Room> Rooms) {
			bool[] Lake = Patch.Generate(Water, 5);
			var Ice = Level is IceLevel;

			foreach (var R in Rooms) {
				foreach (var P in R.WaterPlaceablePoints()) {
					int I = Level.ToIndex((int) P.X, (int) P.Y);
					byte T = Level.Data[I];

					if (Lake[I] && (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C) && Level.LiquidData[I] == 0) {
						Level.Set(I, Ice ? Terrain.ICE : Terrain.WATER);
					}
				}
			}
		}

		private void PaintCobweb(Level Level, List<Room> Rooms) {
			bool[] Lake = Patch.Generate(Water, 5);

			foreach (var R in Rooms) {
				foreach (var P in R.WaterPlaceablePoints()) {
					int I = Level.ToIndex((int) P.X, (int) P.Y);
					byte T = Level.Data[I];

					if (Lake[I] && (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C) && Level.LiquidData[I] == 0) {
						Level.Set(I, Terrain.COBWEB);
					}
				}
			}
		}

		private void PaintDirt(Level Level, List<Room> Rooms) {
			bool[] Grass = Patch.Generate(Dirt, 5);

			foreach (var R in Rooms) {
				foreach (var P in R.GrassPlaceablePoints()) {
					int I = Level.ToIndex((int) P.X, (int) P.Y);
					byte T = Level.Data[I];

					if (Grass[I] && (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C) && Level.LiquidData[I] == 0) {
						Level.Set(I, Terrain.DIRT);
					}
				}
			}
		}

		private void PaintGrass(Level Level, List<Room> Rooms) {
			bool[] Grass = Patch.Generate(this.Grass, 5);
			bool[] Dry = Patch.Generate(this.Grass, 5);
			List<int> Cells = new List<int>();

			foreach (var R in Rooms) {
				foreach (var P in R.GrassPlaceablePoints()) {
					int I = Level.ToIndex((int) P.X, (int) P.Y);
					byte T = Level.Data[I];

					if (Grass[I] && (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C) && Level.LiquidData[I] == 0) {
						Cells.Add(I);
					}
				}
			}

			foreach (int I in Cells) {
				var Count = 1;

				foreach (var N in PathFinder.NEIGHBOURS8) {
					var K = I + N;

					if (Level.IsValid(K) && Grass[K]) {
						Count++;
					}
				}

				var High = Random.Float() < Count / 12f;
				Level.Set(I, High ? Terrain.HIGH_GRASS : Terrain.GRASS);
			}
		}

		protected void Decorate(Level Level, List<Room> Rooms) {
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

		private void PaintDoors(Level Level, List<Room> Rooms) {
			foreach (var R in Rooms) {
				foreach (var N in R.GetConnected().KeySet()) {
					LDoor D = R.GetConnected().Get(N);
					Level.SetDecor((int) D.X, (int) D.Y + 1, (byte) 0);

					if (!(Level is CreepLevel)) {
						byte T = Level.Get((int) D.X, (int) D.Y);
						var Gt = D.GetType() != LDoor.Type.EMPTY && D.GetType() != LDoor.Type.MAZE && D.GetType() != LDoor.Type.TUNNEL && D.GetType() != LDoor.Type.SECRET;

						if (T != Terrain.FLOOR_A && T != Terrain.FLOOR_B && T != Terrain.FLOOR_C && T != Terrain.FLOOR_D && T != Terrain.CRACK && Gt) {
							var Door = new Door((int) D.X, (int) D.Y, !Level.CheckFor((int) D.X + 1, (int) D.Y, Terrain.SOLID));

							if (D.GetType() == LDoor.Type.REGULAR) D.SetType(LDoor.Type.ENEMY);

							Door.AutoLock = D.GetType() == LDoor.Type.ENEMY || D.GetType() == LDoor.Type.BOSS;
							Door.Lock = D.GetType() == LDoor.Type.LEVEL_LOCKED || D.GetType() == LDoor.Type.LOCKED;

							if (D.GetType() == LDoor.Type.LEVEL_LOCKED)
								Door.Key = BurningKey.GetType();
							else if (D.GetType() == LDoor.Type.LOCKED)
								Door.Key = KeyC.GetType();
							else if (D.GetType() == LDoor.Type.BOSS) Door.BkDoor = true;

							Door.Lockable = Door.Lock;
							Door.Add();
							Dungeon.Area.Add(Door);
						}
					}

					if (D.GetType() == LDoor.Type.SECRET) {
						Level.Set((int) D.X, (int) D.Y, Terrain.CRACK);
					} else {
						var F = Terrain.RandomFloor();

						for (var Yy = -1; Yy <= 1; Yy++)
						for (var Xx = -1; Xx <= 1; Xx++)
							if (Math.Abs(Xx) + Math.Abs(Yy) == 1) {
								byte Tl = Level.Get((int) D.X + Xx, (int) D.Y + Yy);

								if (Tl != Terrain.WALL && Tl != Terrain.CRACK && Tl != Terrain.CHASM) {
									F = Tl;

									break;
								}
							}

						Level.Set((int) D.X, (int) D.Y, F);
					}
				}
			}
		}

		private void PlaceDoors(Room R) {
			foreach (Room N in R.GetConnected().KeySet()) {
				LDoor Door = R.GetConnected().Get(N);

				if (Door == null) {
					var I = R.Intersect(N);
					List<Point> DoorSpots = new List<>();

					foreach (Point P in I.GetPoints())
						if (R.CanConnect(P) && N.CanConnect(P))
							DoorSpots.Add(P);

					if (DoorSpots.Size() > 0) {
						Point Point = DoorSpots.Get(Random.NewInt(DoorSpots.Size()));
						Door = new LDoor(Point);
						R.GetConnected().Put(N, Door);
						N.GetConnected().Put(R, Door);
					}
					else {
						R.GetConnected().Remove(N);
						N.GetConnected().Remove(R);

						throw new RuntimeException("Failed to connect rooms " + R.GetClass().GetSimpleName() + " and " + N.GetClass().GetSimpleName());
					}
				}
			}
		}

		public static void Set(Level Level, int Cell, byte Value) {
			Level.Set(Cell, Value);
		}

		public static void Set(Level Level, int X, int Y, byte Value) {
			Set(Level, X + Y * Level.GetWidth(), Value);
		}

		public static void SetBold(Level Level, int X, int Y, byte Value) {
			for (var Yy = Y - 1; Yy < Y + 2; Yy++)
			for (var Xx = X - 1; Xx < X + 2; Xx++) {
				byte T = Level.Get(Xx, Yy);

				if (T != Value) {
					if (Xx != X || Yy != Y)
						if (T == Terrain.WALL)
							continue;

					Set(Level, Xx, Yy, Value);
				}
			}
		}

		public static void Set(Level Level, Point P, byte Value) {
			Set(Level, (int) P.X, (int) P.Y, Value);
		}

		public static void Fill(Level Level, int X, int Y, int W, int H, byte Value) {
			for (var Yy = Y; Yy < Y + H; Yy++)
			for (var Xx = X; Xx < X + W; Xx++)
				Set(Level, Xx, Yy, Value);
		}

		public static void Triangle(Level Level, Point From, Point P1, Point P2, byte V) {
			if (P1.X != P2.X)
				for (var X = (int) P1.X; X < P2.X; X++)
					DrawLine(Level, From, new Point(X, P1.Y), V);
			else
				for (var Y = (int) P1.Y; Y < P2.Y; Y++)
					DrawLine(Level, From, new Point(P1.X, Y), V);
		}

		public static void Fill(Level Level, Rect Rect, byte Value) {
			Fill(Level, Rect.Left, Rect.Top, Rect.GetWidth(), Rect.GetHeight(), Value);
		}

		public static void Fill(Level Level, Rect Rect, int M, byte Value) {
			Fill(Level, Rect.Left + M, Rect.Top + M, Rect.GetWidth() - M * 2, Rect.GetHeight() - M * 2, Value);
		}

		public static void Fill(Level Level, Rect Rect, int L, int T, int R, int B, byte Value) {
			Fill(Level, Rect.Left + L, Rect.Top + T, Rect.GetWidth() - (L + R), Rect.GetHeight() - (T + B), Value);
		}

		public static void DrawLine(Level Level, Point From, Point To, byte Value) {
			DrawLine(Level, From, To, Value, false);
		}

		public static void DrawLine(Level Level, Point From, Point To, byte Value, bool Bold) {
			float X = From.X;
			float Y = From.Y;
			float Dx = To.X - From.X;
			float Dy = To.Y - From.Y;
			bool MovingbyX = Math.Abs(Dx) >= Math.Abs(Dy);

			if (MovingbyX) {
				Dy /= Math.Abs(Dx);
				Dx /= Math.Abs(Dx);
			}
			else {
				Dx /= Math.Abs(Dy);
				Dy /= Math.Abs(Dy);
			}


			if (Bold)
				SetBold(Level, Math.Round(X), Math.Round(Y), Value);
			else
				Set(Level, Math.Round(X), Math.Round(Y), Value);


			while (MovingbyX && To.X != X || !MovingbyX && To.Y != Y) {
				X += Dx;
				Y += Dy;

				if (Bold)
					SetBold(Level, Math.Round(X), Math.Round(Y), Value);
				else
					Set(Level, Math.Round(X), Math.Round(Y), Value);
			}
		}

		public static void FillEllipse(Level Level, Rect Rect, byte Value) {
			FillEllipse(Level, Rect.Left, Rect.Top, Rect.GetWidth(), Rect.GetHeight(), Value);
		}

		public static void FillEllipse(Level Level, Rect Rect, int M, byte Value) {
			FillEllipse(Level, Rect.Left + M, Rect.Top + M, Rect.GetWidth() - M * 2, Rect.GetHeight() - M * 2, Value);
		}

		public static void FillEllipse(Level Level, int X, int Y, int W, int H, byte Value) {
			double RadH = H / 2f;
			double RadW = W / 2f;
			bool Liquid = Level.MatchesFlag(Value, Terrain.LIQUID_LAYER);

			for (var I = 0; I < H; I++) {
				var RowY = -RadH + 0.5 + I;
				var RowW = 2.0 * Math.Sqrt(RadW * RadW * (1.0 - RowY * RowY / (RadH * RadH)));

				if (W % 2 == 0) {
					RowW = Math.Round(RowW / 2.0) * 2.0;
				}
				else {
					RowW = Math.Floor(RowW / 2.0) * 2.0;
					RowW++;
				}


				var Cell = X + (W - (int) RowW) / 2 + (Y + I) * Level.GetWidth();

				for (var J = Cell; J < Cell + RowW; J++) Level.Set(J, Value);
			}
		}

		public static Point DrawInside(Level Level, Room Room, Point From, int N, byte Value) {
			var Step = new Point();

			if (From.X == Room.Left)
				Step.Set(+1, 0);
			else if (From.X == Room.Right)
				Step.Set(-1, 0);
			else if (From.Y == Room.Top)
				Step.Set(0, +1);
			else if (From.Y == Room.Bottom) Step.Set(0, -1);

			var P = new Point(From).Offset(Step);

			for (var I = 0; I < N; I++) {
				if (Value != -1) Set(Level, P, Value);

				P.Offset(Step);
			}

			return P;
		}
	}
}