using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms {
	public abstract class Room : Rect {
		enum Connection {
			ALL,
			LEFT,
			RIGHT,
			TOP,
			BOTTOM
		}

		public List<Room> Neighbours = new List<>();
		public Dictionary<Room, LDoor> Connected = new Dictionary<>();
		private int Price = 1;
		private int Distance = 0;
		public bool Hidden;
		public int NumEnemies;
		public int LastNumEnemies;
		public int Id;

		public int GetMinWidth() {
			return 10;
		}

		public int GetMinHeight() {
			return 10;
		}

		public int GetMaxWidth() {
			return 16;
		}

		public int GetMaxHeight() {
			return 16;
		}

		public abstract int GetMaxConnections(Connection Side);

		public abstract int GetMinConnections(Connection Side);

		public Void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}

		public int GetCurrentConnections(Connection Direction) {
			if (Direction == Connection.ALL) {
				return Connected.Size();
			} else {
				int Total = 0;

				foreach (Room R in Connected.KeySet()) {
					Rect I = Intersect(R);

					if (Direction == Connection.LEFT && I.GetWidth() == 0 && I.Left == Left) {
						Total++;
					} else if (Direction == Connection.TOP && I.GetHeight() == 0 && I.Top == Top) {
						Total++;
					} else if (Direction == Connection.RIGHT && I.GetWidth() == 0 && I.Right == Right) {
						Total++;
					} else if (Direction == Connection.BOTTOM && I.GetHeight() == 0 && I.Bottom == Bottom) {
						Total++;
					} 
				}

				return Total;
			}

		}

		public int GetLastConnections(Connection Direction) {
			if (this.GetCurrentConnections(Connection.ALL) >= this.GetMaxConnections(Connection.ALL)) {
				return 0;
			} else {
				return this.GetMaxConnections(Direction) - this.GetCurrentConnections(Direction);
			}

		}

		public bool CanConnect(Point P) {
			return (P.X == Left || P.X == Right) != (P.Y == Top || P.Y == Bottom);
		}

		public bool CanConnect(Connection Direction) {
			int Cnt = this.GetLastConnections(Direction);

			return Cnt > 0;
		}

		public bool CanConnect(Room R) {
			Rect I = this.Intersect(R);
			bool FoundPoint = false;

			foreach (Point P in I.GetPoints()) {
				if (this.CanConnect(P) && R.CanConnect(P)) {
					FoundPoint = true;

					break;
				} 
			}

			if (!FoundPoint) {
				return false;
			} 

			if (I.GetWidth() == 0 && I.Left == Left) {
				return this.CanConnect(Connection.LEFT) && R.CanConnect(Connection.LEFT);
			} else if (I.GetHeight() == 0 && I.Top == Top) {
				return this.CanConnect(Connection.TOP) && R.CanConnect(Connection.TOP);
			} else if (I.GetWidth() == 0 && I.Right == Right) {
				return this.CanConnect(Connection.RIGHT) && R.CanConnect(Connection.RIGHT);
			} else if (I.GetHeight() == 0 && I.Bottom == Bottom) {
				return this.CanConnect(Connection.BOTTOM) && R.CanConnect(Connection.BOTTOM);
			} else {
				return false;
			}

		}

		public bool ConnectTo(Room Other) {
			if (this.Neighbours.Contains(Other)) {
				return true;
			} 

			Rect I = this.Intersect(Other);
			int W = I.GetWidth();
			int H = I.GetHeight();

			if ((W == 0 && H >= 2) || (H == 0 && W >= 2)) {
				this.Neighbours.Add(Other);
				Other.Neighbours.Add(this);

				return true;
			} 

			return false;
		}

		public bool ConnectWithRoom(Room Room) {
			if ((Neighbours.Contains(Room) || ConnectTo(Room)) && !Connected.ContainsKey(Room) && CanConnect(Room)) {
				Connected.Put(Room, null);
				Room.Connected.Put(this, null);

				return true;
			} 

			return false;
		}

		public Point GetRandomCell() {
			int X = Random.NewInt(this.Left + 1, this.Right);
			int Y = Random.NewInt(this.Top + 1, this.Bottom);

			return new Point(X, Y);
		}

		public Point GetRandomFreeCell() {
			Point Point;
			int At = 0;

			do {
				if (At++ > 200) {
					Log.Error("To many attempts");

					return null;
				} 

				Point = GetRandomCell();
			} while (!Dungeon.Level.CheckFor((int) Point.X, (int) Point.Y, Terrain.PASSABLE));

			return Point;
		}

		public Point GetRandomDoorFreeCell() {
			Point Point;
			int At = 0;

			while (true) {
				if (At++ > 200) {
					Log.Error("To many attempts");

					return null;
				} 

				Point = GetRandomCell();

				if (Connected.Size() == 0) {
					return Point;
				} 

				foreach (LDoor Door in Connected.Values()) {
					int Dx = (int) (Door.X - Point.X);
					int Dy = (int) (Door.Y - Point.Y);
					float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

					if (D > 3 && Dungeon.Level.CheckFor((int) Point.X, (int) Point.Y, Terrain.PASSABLE)) {
						return Point;
					} 
				}
			}
		}

		public Room GetRandomNeighbour() {
			return this.Neighbours.Get(Random.NewInt(this.Neighbours.Size()));
		}

		public Dictionary GetConnected<Room, LDoor> () {
			return this.Connected;
		}

		public List GetNeighbours<Room> () {
			return this.Neighbours;
		}

		public bool SetSize() {
			return SetSize(this.GetMinWidth(), this.GetMaxWidth(), this.GetMinHeight(), this.GetMaxHeight());
		}

		protected int ValidateWidth(int W) {
			return W;
		}

		protected int ValidateHeight(int H) {
			return H;
		}

		protected bool SetSize(int MinW, int MaxW, int MinH, int MaxH) {
			if (MinW < this.GetMinWidth() || MaxW > this.GetMaxWidth() || MinH < this.GetMinHeight() || MaxH > this.GetMaxHeight() || MinW > MaxW || MinH > MaxH) {
				return false;
			} else {
				if (Quad()) {
					int V = Math.Min(ValidateWidth(Random.NewInt(MinW, MaxW) - 1), ValidateHeight(Random.NewInt(MinH, MaxH) - 1));
					this.Resize(V, V);
				} else {
					this.Resize(ValidateWidth(Random.NewInt(MinW, MaxW) - 1), ValidateHeight(Random.NewInt(MinH, MaxH) - 1));
				}


				return true;
			}

		}

		protected bool Quad() {
			return false;
		}

		public bool SetSizeWithLimit(int W, int H) {
			if (W < this.GetMinWidth() || H < this.GetMinHeight()) {
				return false;
			} else {
				SetSize();

				if (GetWidth() > W || GetHeight() > H) {
					int Ww = ValidateWidth(Math.Min(GetWidth(), W) - 1);
					int Hh = ValidateHeight(Math.Min(GetHeight(), H) - 1);

					if (Ww >= W || Hh >= H) {
						return false;
					} 

					Resize(Ww, Hh);
				} 

				return true;
			}

		}

		public Void ClearConnections() {
			foreach (Room R in this.Neighbours) {
				R.Neighbours.Remove(this);
			}

			this.Neighbours.Clear();

			foreach (Room R in this.Connected.KeySet()) {
				R.Connected.Remove(this);
			}

			this.Connected.Clear();
		}

		public bool CanPlaceWater(Point P) {
			return Inside(P);
		}

		public const List WaterPlaceablePoints<Point> () {
			List<Point> Points = new List<>();

			for (int I = Left + 1; I <= Right - 1; I++) {
				for (int J = Top + 1; J <= Bottom - 1; J++) {
					Point P = new Point(I, J);

					if (CanPlaceWater(P)) Points.Add(P);

				}
			}

			return Points;
		}

		public bool CanPlaceGrass(Point P) {
			return Inside(P);
		}

		public const List GrassPlaceablePoints<Point> () {
			List<Point> Points = new List<>();

			for (int I = Left + 1; I <= Right - 1; I++) {
				for (int J = Top + 1; J <= Bottom - 1; J++) {
					Point P = new Point(I, J);

					if (CanPlaceGrass(P)) Points.Add(P);

				}
			}

			return Points;
		}

		public override int GetWidth() {
			return base.GetWidth() + 1;
		}

		public override int GetHeight() {
			return base.GetHeight() + 1;
		}

		public Point GetCenter() {
			return new Point(this.Left + this.GetWidth() / 2, this.Top + this.GetHeight() / 2);
		}

		protected Rect GetConnectionSpace() {
			Point C = GetDoorCenter();

			return new Rect((int) C.X, (int) C.Y, (int) C.X, (int) C.Y);
		}

		protected Point GetDoorCenter() {
			Point DoorCenter = new Point(0, 0);

			foreach (LDoor Door in Connected.Values()) {
				DoorCenter.X += Door.X;
				DoorCenter.Y += Door.Y;
			}

			int N = this.Connected.Size();
			Point C = new Point((int) DoorCenter.X / N, (int) DoorCenter.Y / N);

			if (Random.NewFloat() < DoorCenter.X % 1) C.X++;


			if (Random.NewFloat() < DoorCenter.Y % 1) C.Y++;


			C.X = (int) MathUtils.Clamp(Left + 1, Right - 1, C.X);
			C.Y = (int) MathUtils.Clamp(Top + 1, Bottom - 1, C.Y);

			return C;
		}

		protected Void PaintTunnel(Level Level, byte Floor) {
			PaintTunnel(Level, Floor, false);
		}

		protected Void PaintTunnel(Level Level, byte Floor, bool Bold) {
			if (this.Connected.Size() == 0) {
				Log.Error("Invalid connection room");

				return;
			} 

			Rect C = GetConnectionSpace();

			foreach (LDoor Door in this.GetConnected().Values()) {
				Point Start;
				Point Mid;
				Point End;
				Start = new Point(Door);

				if (Start.X == Left) Start.X++;
else if (Start.Y == Top) Start.Y++;
else if (Start.X == Right) Start.X--;
else if (Start.Y == Bottom) Start.Y--;


				int RightShift;
				int DownShift;

				if (Start.X < C.Left) RightShift = (int) (C.Left - Start.X);
else if (Start.X > C.Right) RightShift = (int) (C.Right - Start.X);
else RightShift = 0;


				if (Start.Y < C.Top) DownShift = (int) (C.Top - Start.Y);
else if (Start.Y > C.Bottom) DownShift = (int) (C.Bottom - Start.Y);
else DownShift = 0;


				if (Door.X == Left || Door.X == Right) {
					Mid = new Point(Start.X + RightShift, Start.Y);
					End = new Point(Mid.X, Mid.Y + DownShift);
				} else {
					Mid = new Point(Start.X, Start.Y + DownShift);
					End = new Point(Mid.X + RightShift, Mid.Y);
				}


				Painter.DrawLine(Level, Start, Mid, Floor, Bold);
				Painter.DrawLine(Level, Mid, End, Floor, Bold);
			}
		}
	}
}
