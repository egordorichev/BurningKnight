using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.entity.item.key;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.entities.decor;
using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.levels.creep;
using BurningKnight.core.entity.level.levels.forest;
using BurningKnight.core.entity.level.levels.hall;
using BurningKnight.core.entity.level.levels.ice;
using BurningKnight.core.entity.level.levels.tech;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.entity.level.rooms.entrance;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;
using Door = BurningKnight.core.entity.level.entities.Door;

namespace BurningKnight.core.entity.level.painters {
	public class Painter {
		private float Grass = 0f;
		private float Dirt = 0f;
		private float Water = 0f;
		private float Cobweb = 0f;

		public Painter SetWater(float V) {
			this.Water = V;

			return this;
		}

		public Painter SetCobweb(float V) {
			this.Cobweb = V;

			return this;
		}

		public Painter SetDirt(float V) {
			this.Dirt = V;

			return this;
		}

		public Painter SetGrass(float V) {
			this.Grass = V;

			return this;
		}

		public Void Paint(Level Level, List Rooms) {
			if (Rooms == null) {
				return;
			} 

			int LeftMost = Integer.MAX_VALUE;
			int TopMost = Integer.MAX_VALUE;

			foreach (Room R in Rooms) {
				if (R.Left < LeftMost) LeftMost = R.Left;


				if (R.Top < TopMost) TopMost = R.Top;

			}

			LeftMost--;
			TopMost--;
			int Sz = 0;
			LeftMost -= Sz;
			TopMost -= Sz;
			int RightMost = 0;
			int BottomMost = 0;

			foreach (Room R in Rooms) {
				R.Shift(-LeftMost, -TopMost);

				if (R.Right > RightMost) RightMost = R.Right;


				if (R.Bottom > BottomMost) BottomMost = R.Bottom;

			}

			RightMost++;
			BottomMost++;
			RightMost += Sz;
			BottomMost += Sz;
			Log.Info("Setting level size to " + (1 + RightMost) + ":" + (BottomMost + 1));
			Level.SetSize(RightMost + 1, BottomMost + 1);
			Level.GenerateDecor();
			Level.Fill();

			foreach (Room Room in Rooms) {
				this.PlaceDoors(Room);
				Room.Paint(Level);

				if ((Level is HallLevel || Level is IceLevel) && Dungeon.Depth > -1) {
					for (int Y = Room.Top; Y <= Room.Bottom; Y++) {
						for (int X = Room.Left; X <= Room.Right; X++) {
							int I = Level.ToIndex(X, Y);

							if (Level.LiquidData[I] == Terrain.LAVA) {
								Level.LiquidData[I] = 0;
								Level.Set(I, Terrain.CHASM);
							} 
						}
					}
				} 

				if ((Level is IceLevel)) {
					for (int Y = Room.Top; Y <= Room.Bottom; Y++) {
						for (int X = Room.Left; X <= Room.Right; X++) {
							int I = Level.ToIndex(X, Y);

							if (Level.LiquidData[I] == Terrain.WATER) {
								Level.LiquidData[I] = 0;
								Level.Set(I, Terrain.ICE);
							} 
						}
					}
				} 

				if (!(Room is BossEntranceRoom) && Level is ForestLevel && Random.Chance(70)) {
					for (int I = 0; I < Random.NewInt(1, 4); I++) {
						Point Point = Room.GetRandomFreeCell();

						if (Point != null) {
							Bush Bush = new Bush();
							Bush.X = Point.X * 16 + Random.NewFloat(-4, 4);
							Bush.Y = Point.Y * 16 + Random.NewFloat(-4, 4);
							Dungeon.Area.Add(Bush.Add());
						} 
					}
				} 

				if (Room.Hidden) {
					for (int Y = Room.Top; Y <= Room.Bottom; Y++) {
						for (int X = Room.Left; X <= Room.Right; X++) {
							Level.Hide(X, Y);
						}
					}
				} 
			}

			if (PathFinder.NEIGHBOURS8 == null) {
				PathFinder.SetMapSize(Level.GetWidth(), Level.GetHeight());
			} 

			if (Dungeon.Depth > -1) {
				if (this.Dirt > 0) {
					this.PaintDirt(Level, Rooms);
				} 

				if (this.Grass > 0) {
					this.PaintGrass(Level, Rooms);
				} 

				if (this.Cobweb > 0) {
					this.PaintCobweb(Level, Rooms);
				} 

				if (this.Water > 0) {
					this.PaintWater(Level, Rooms);
				} 
			} 

			this.Decorate(Level, Rooms);
			this.PaintDoors(Level, Rooms);
		}

		private Void PaintWater(Level Level, List Rooms) {
			bool[] Lake = Patch.Generate(this.Water, 5);
			bool Ice = Level is IceLevel;

			foreach (Room R in Rooms) {
				foreach (Point P in R.WaterPlaceablePoints()) {
					int I = Level.ToIndex((int) P.X, (int) P.Y);
					byte T = Level.Data[I];

					if (Lake[I] && (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C) && Level.LiquidData[I] == 0) {
						Level.Set(I, Ice ? Terrain.ICE : Terrain.WATER);
					} 
				}
			}
		}

		private Void PaintCobweb(Level Level, List Rooms) {
			bool[] Lake = Patch.Generate(this.Water, 5);

			foreach (Room R in Rooms) {
				foreach (Point P in R.WaterPlaceablePoints()) {
					int I = Level.ToIndex((int) P.X, (int) P.Y);
					byte T = Level.Data[I];

					if (Lake[I] && (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C) && Level.LiquidData[I] == 0) {
						Level.Set(I, Terrain.COBWEB);
					} 
				}
			}
		}

		private Void PaintDirt(Level Level, List Rooms) {
			bool[] Grass = Patch.Generate(this.Dirt, 5);

			foreach (Room R in Rooms) {
				foreach (Point P in R.GrassPlaceablePoints()) {
					int I = Level.ToIndex((int) P.X, (int) P.Y);
					byte T = Level.Data[I];

					if (Grass[I] && (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C) && Level.LiquidData[I] == 0) {
						Level.Set(I, Terrain.DIRT);
					} 
				}
			}
		}

		private Void PaintGrass(Level Level, List Rooms) {
			bool[] Grass = Patch.Generate(this.Grass, 5);
			bool[] Dry = Patch.Generate(this.Grass, 5);
			List<Integer> Cells = new List<>();

			foreach (Room R in Rooms) {
				foreach (Point P in R.GrassPlaceablePoints()) {
					int I = Level.ToIndex((int) P.X, (int) P.Y);
					byte T = Level.Data[I];

					if (Grass[I] && (T == Terrain.FLOOR_A || T == Terrain.FLOOR_B || T == Terrain.FLOOR_C) && Level.LiquidData[I] == 0) {
						Cells.Add(I);
					} 
				}
			}

			foreach (int I in Cells) {
				int Count = 1;

				foreach (int N in PathFinder.NEIGHBOURS8) {
					int K = I + N;

					if (Level.IsValid(K) && Grass[K]) {
						Count++;
					} 
				}

				bool High = (Random.NewFloat() < Count / 12f);
				Level.Set(I, false ? (High ? Terrain.HIGH_DRY_GRASS : Terrain.DRY_GRASS) : (High ? Terrain.HIGH_GRASS : Terrain.GRASS));
			}
		}

		protected Void Decorate(Level Level, List Rooms) {
			foreach (Room Room in Rooms) {
				if (Random.Chance(60)) {
					for (int I = 0; I < (Random.Chance(50) ? 1 : Random.NewInt(3, 6)); I++) {
						Firefly Fly = new Firefly();
						Fly.X = (Room.Left + 2) * 16 + Random.NewFloat((Room.GetWidth() - 4) * 16);
						Fly.Y = (Room.Top + 2) * 16 + Random.NewFloat((Room.GetHeight() - 4) * 16);
						Dungeon.Area.Add(Fly.Add());
					}
				} 

				for (int Y = Room.Top; Y <= Room.Bottom; Y++) {
					for (int X = Room.Left; X <= Room.Right; X++) {
						if (Dungeon.Depth > -2 && Level.Get(X, Y) == Terrain.WALL) {
							if (Random.Chance(30)) {
								Level.SetDecor(X, Y, (byte) (Random.NewInt(Terrain.Decor.Length) + 1));
							} 
						} 

						if (Dungeon.Depth > -1 && Level.Get(X, Y) == Terrain.WALL && !(Level is IceLevel || Level is TechLevel)) {
							if (Y > Room.Top && X > Room.Left && Level.Get(X - 1, Y - 1) == Terrain.WALL && Level.Get(X, Y - 1) != Terrain.WALL && Random.Chance(20)) {
								Cobweb Web = new Cobweb();
								Web.X = X * 16;
								Web.Y = Y * 16 - 16;
								Web.Side = 0;
								Dungeon.Area.Add(Web);
								LevelSave.Add(Web);
							} else if (Y > Room.Top && X < Room.Right && Level.Get(X + 1, Y - 1) == Terrain.WALL && Level.Get(X, Y - 1) != Terrain.WALL && Random.Chance(20)) {
								Cobweb Web = new Cobweb();
								Web.X = X * 16;
								Web.Y = Y * 16 - 16;
								Web.Side = 1;
								Dungeon.Area.Add(Web);
								LevelSave.Add(Web);
							} else if (Y < Room.Bottom - 1 && X > Room.Left && Level.Get(X - 1, Y + 1) == Terrain.WALL && Level.Get(X, Y + 1) != Terrain.WALL && Random.Chance(20)) {
								Cobweb Web = new Cobweb();
								Web.X = X * 16;
								Web.Y = Y * 16 + 16;
								Web.Side = 2;
								Dungeon.Area.Add(Web);
								LevelSave.Add(Web);
							} else if (Y < Room.Bottom - 1 && X < Room.Right && Level.Get(X + 1, Y + 1) == Terrain.WALL && Level.Get(X, Y + 1) != Terrain.WALL && Random.Chance(20)) {
								Cobweb Web = new Cobweb();
								Web.X = X * 16;
								Web.Y = Y * 16 + 16;
								Web.Side = 3;
								Dungeon.Area.Add(Web);
								LevelSave.Add(Web);
							} 
						} 
					}
				}
			}
		}

		private Void PaintDoors(Level Level, List Rooms) {
			foreach (Room R in Rooms) {
				foreach (Room N in R.GetConnected().KeySet()) {
					LDoor D = R.GetConnected().Get(N);
					Level.SetDecor((int) D.X, (int) D.Y + 1, (byte) 0);

					if (!(Level is CreepLevel)) {
						byte T = Level.Get((int) D.X, (int) D.Y);
						bool Gt = (D.GetType() != LDoor.Type.EMPTY && D.GetType() != LDoor.Type.MAZE && D.GetType() != LDoor.Type.TUNNEL && D.GetType() != LDoor.Type.SECRET);

						if (T != Terrain.FLOOR_A && T != Terrain.FLOOR_B && T != Terrain.FLOOR_C && T != Terrain.FLOOR_D && T != Terrain.CRACK && Gt) {
							Door Door = new Door((int) D.X, (int) D.Y, !Level.CheckFor((int) D.X + 1, (int) D.Y, Terrain.SOLID));

							if (D.GetType() == LDoor.Type.REGULAR) {
								D.SetType(LDoor.Type.ENEMY);
							} 

							Door.AutoLock = (D.GetType() == LDoor.Type.ENEMY || D.GetType() == LDoor.Type.BOSS);
							Door.Lock = (D.GetType() == LDoor.Type.LEVEL_LOCKED || D.GetType() == LDoor.Type.LOCKED);

							if (D.GetType() == LDoor.Type.LEVEL_LOCKED) {
								Door.Key = BurningKey.GetType();
							} else if (D.GetType() == LDoor.Type.LOCKED) {
								Door.Key = KeyC.GetType();
							} else if (D.GetType() == LDoor.Type.BOSS) {
								Door.BkDoor = true;
							} 

							Door.Lockable = Door.Lock;
							Door.Add();
							Dungeon.Area.Add(Door);
						} 
					} 

					if (D.GetType() == LDoor.Type.SECRET) {
						Level.Set((int) D.X, (int) D.Y, Terrain.CRACK);
					} else {
						byte F = Terrain.RandomFloor();

						for (int Yy = -1; Yy <= 1; Yy++) {
							for (int Xx = -1; Xx <= 1; Xx++) {
								if (Math.Abs(Xx) + Math.Abs(Yy) == 1) {
									byte Tl = Level.Get((int) D.X + Xx, (int) D.Y + Yy);

									if (Tl != Terrain.WALL && Tl != Terrain.CRACK && Tl != Terrain.CHASM) {
										F = Tl;

										break;
									} 
								} 
							}
						}

						Level.Set((int) D.X, (int) D.Y, F);
					}

				}
			}
		}

		private Void PlaceDoors(Room R) {
			foreach (Room N in R.GetConnected().KeySet()) {
				LDoor Door = R.GetConnected().Get(N);

				if (Door == null) {
					Rect I = R.Intersect(N);
					List<Point> DoorSpots = new List<>();

					foreach (Point P in I.GetPoints()) {
						if (R.CanConnect(P) && N.CanConnect(P)) {
							DoorSpots.Add(P);
						} 
					}

					if (DoorSpots.Size() > 0) {
						Point Point = DoorSpots.Get(Random.NewInt(DoorSpots.Size()));
						Door = new LDoor(Point);
						R.GetConnected().Put(N, Door);
						N.GetConnected().Put(R, Door);
					} else {
						R.GetConnected().Remove(N);
						N.GetConnected().Remove(R);

						throw new RuntimeException("Failed to connect rooms " + R.GetClass().GetSimpleName() + " and " + N.GetClass().GetSimpleName());
					}

				} 
			}
		}

		public static Void Set(Level Level, int Cell, byte Value) {
			Level.Set(Cell, Value);
		}

		public static Void Set(Level Level, int X, int Y, byte Value) {
			Set(Level, X + Y * Level.GetWidth(), Value);
		}

		public static Void SetBold(Level Level, int X, int Y, byte Value) {
			for (int Yy = Y - 1; Yy < Y + 2; Yy++) {
				for (int Xx = X - 1; Xx < X + 2; Xx++) {
					byte T = Level.Get(Xx, Yy);

					if (T != Value) {
						if (Xx != X || Yy != Y) {
							if (T == Terrain.WALL) {
								continue;
							} 
						} 

						Set(Level, Xx, Yy, Value);
					} 
				}
			}
		}

		public static Void Set(Level Level, Point P, byte Value) {
			Set(Level, (int) P.X, (int) P.Y, Value);
		}

		public static Void Fill(Level Level, int X, int Y, int W, int H, byte Value) {
			for (int Yy = Y; Yy < Y + H; Yy++) {
				for (int Xx = X; Xx < X + W; Xx++) {
					Set(Level, Xx, Yy, Value);
				}
			}
		}

		public static Void Triangle(Level Level, Point From, Point P1, Point P2, byte V) {
			if (P1.X != P2.X) {
				for (int X = (int) P1.X; X < P2.X; X++) {
					DrawLine(Level, From, new Point(X, P1.Y), V);
				}
			} else {
				for (int Y = (int) P1.Y; Y < P2.Y; Y++) {
					DrawLine(Level, From, new Point(P1.X, Y), V);
				}
			}

		}

		public static Void Fill(Level Level, Rect Rect, byte Value) {
			Fill(Level, Rect.Left, Rect.Top, Rect.GetWidth(), Rect.GetHeight(), Value);
		}

		public static Void Fill(Level Level, Rect Rect, int M, byte Value) {
			Fill(Level, Rect.Left + M, Rect.Top + M, Rect.GetWidth() - M * 2, Rect.GetHeight() - M * 2, Value);
		}

		public static Void Fill(Level Level, Rect Rect, int L, int T, int R, int B, byte Value) {
			Fill(Level, Rect.Left + L, Rect.Top + T, Rect.GetWidth() - (L + R), Rect.GetHeight() - (T + B), Value);
		}

		public static Void DrawLine(Level Level, Point From, Point To, byte Value) {
			DrawLine(Level, From, To, Value, false);
		}

		public static Void DrawLine(Level Level, Point From, Point To, byte Value, bool Bold) {
			float X = From.X;
			float Y = From.Y;
			float Dx = To.X - From.X;
			float Dy = To.Y - From.Y;
			bool MovingbyX = Math.Abs(Dx) >= Math.Abs(Dy);

			if (MovingbyX) {
				Dy /= Math.Abs(Dx);
				Dx /= Math.Abs(Dx);
			} else {
				Dx /= Math.Abs(Dy);
				Dy /= Math.Abs(Dy);
			}


			if (Bold) {
				SetBold(Level, Math.Round(X), Math.Round(Y), Value);
			} else {
				Set(Level, Math.Round(X), Math.Round(Y), Value);
			}


			while ((MovingbyX && To.X != X) || (!MovingbyX && To.Y != Y)) {
				X += Dx;
				Y += Dy;

				if (Bold) {
					SetBold(Level, Math.Round(X), Math.Round(Y), Value);
				} else {
					Set(Level, Math.Round(X), Math.Round(Y), Value);
				}

			}
		}

		public static Void FillEllipse(Level Level, Rect Rect, byte Value) {
			FillEllipse(Level, Rect.Left, Rect.Top, Rect.GetWidth(), Rect.GetHeight(), Value);
		}

		public static Void FillEllipse(Level Level, Rect Rect, int M, byte Value) {
			FillEllipse(Level, Rect.Left + M, Rect.Top + M, Rect.GetWidth() - M * 2, Rect.GetHeight() - M * 2, Value);
		}

		public static Void FillEllipse(Level Level, int X, int Y, int W, int H, byte Value) {
			double RadH = H / 2f;
			double RadW = W / 2f;
			bool Liquid = Level.MatchesFlag(Value, Terrain.LIQUID_LAYER);

			for (int I = 0; I < H; I++) {
				double RowY = -RadH + 0.5 + I;
				double RowW = 2.0 * Math.Sqrt((RadW * RadW) * (1.0 - (RowY * RowY) / (RadH * RadH)));

				if (W % 2 == 0) {
					RowW = Math.Round(RowW / 2.0) * 2.0;
				} else {
					RowW = Math.Floor(RowW / 2.0) * 2.0;
					RowW++;
				}


				int Cell = X + (W - (int) RowW) / 2 + ((Y + I) * Level.GetWidth());

				for (int J = Cell; J < Cell + RowW; J++) {
					Level.Set(J, Value);
				}
			}
		}

		public static Point DrawInside(Level Level, Room Room, Point From, int N, byte Value) {
			Point Step = new Point();

			if (From.X == Room.Left) {
				Step.Set(+1, 0);
			} else if (From.X == Room.Right) {
				Step.Set(-1, 0);
			} else if (From.Y == Room.Top) {
				Step.Set(0, +1);
			} else if (From.Y == Room.Bottom) {
				Step.Set(0, -1);
			} 

			Point P = new Point(From).Offset(Step);

			for (int I = 0; I < N; I++) {
				if (Value != -1) {
					Set(Level, P, Value);
				} 

				P.Offset(Step);
			}

			return P;
		}
	}
}
