using System;
using System.Collections.Generic;
using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.level.rooms {
	public abstract class Room : Rect {
		public enum Connection {
			ALL,
			LEFT,
			RIGHT,
			TOP,
			BOTTOM
		}

		public Dictionary<Room, DoorPlaceholder> Connected = new Dictionary<Room, DoorPlaceholder>();
		private int Distance = 0;
		public bool Hidden;
		public int Id;

		public List<Room> Neighbours = new List<Room>();

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

		public void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());

			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Regular;
			}
		}

		public int GetCurrentConnections(Connection Direction) {
			if (Direction == Connection.ALL) {
				return Connected.Count;
			}

			var Total = 0;

			foreach (var R in Connected.Keys) {
				var I = Intersect(R);

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

		public int GetLastConnections(Connection Direction) {
			if (GetCurrentConnections(Connection.ALL) >= GetMaxConnections(Connection.ALL)) {
				return 0;
			}

			return GetMaxConnections(Direction) - GetCurrentConnections(Direction);
		}

		public bool CanConnect(Vector2 P) {
			return ((int) P.X == Left || (int) P.X == Right) != ((int) P.Y == Top || (int) P.Y == Bottom);
		}

		public bool CanConnect(Connection Direction) {
			var Cnt = GetLastConnections(Direction);

			return Cnt > 0;
		}

		public bool CanConnect(Room R) {
			var I = Intersect(R);
			var FoundPoint = false;

			foreach (var P in I.GetPoints()) {
				if (CanConnect(P) && R.CanConnect(P)) {
					FoundPoint = true;

					break;
				}
			}

			if (!FoundPoint) {
				return false;
			}

			if (I.GetWidth() == 0 && I.Left == Left) {
				return CanConnect(Connection.LEFT) && R.CanConnect(Connection.LEFT);
			}

			if (I.GetHeight() == 0 && I.Top == Top) {
				return CanConnect(Connection.TOP) && R.CanConnect(Connection.TOP);
			}

			if (I.GetWidth() == 0 && I.Right == Right) {
				return CanConnect(Connection.RIGHT) && R.CanConnect(Connection.RIGHT);
			}

			if (I.GetHeight() == 0 && I.Bottom == Bottom) {
				return CanConnect(Connection.BOTTOM) && R.CanConnect(Connection.BOTTOM);
			}

			return false;
		}

		public bool ConnectTo(Room Other) {
			if (Neighbours.Contains(Other)) {
				return true;
			}

			var I = Intersect(Other);
			var W = I.GetWidth();
			var H = I.GetHeight();

			if (W == 0 && H >= 2 || H == 0 && W >= 2) {
				Neighbours.Add(Other);
				Other.Neighbours.Add(this);

				return true;
			}

			return false;
		}

		public bool ConnectWithRoom(Room Room) {
			if ((Neighbours.Contains(Room) || ConnectTo(Room)) && !Connected.ContainsKey(Room) && CanConnect(Room)) {
				Connected[Room] = null;
				Room.Connected[this] = null;

				return true;
			}

			return false;
		}

		public Point GetRandomCell() {
			var X = Random.Int(Left + 1, Right);
			var Y = Random.Int(Top + 1, Bottom);

			return new Point(X, Y);
		}

		public Point? GetRandomFreeCell() {
			Point Point;
			var At = 0;

			do {
				if (At++ > 200) {
					Log.Error("To many attempts");

					return null;
				}

				Point = GetRandomCell();
			} while (!Dungeon.Level.CheckFor(Point.X, Point.Y, Terrain.PASSABLE));

			return Point;
		}

		public Vector2 GetRandomDoorFreeCell() {
			Vector2 Point;
			var At = 0;

			while (true) {
				if (At++ > 200) {
					Log.Error("To many attempts");

					return null;
				}

				Point = GetRandomCell();

				if (Connected.Count == 0) {
					return Point;
				}

				foreach (var Door in Connected.Values) {
					var Dx = (int) (Door.X - Point.X);
					var Dy = (int) (Door.Y - Point.Y);
					var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

					if (D > 3 && Dungeon.Level.CheckFor((int) Point.X, (int) Point.Y, Terrain.PASSABLE)) {
						return Point;
					}
				}
			}
		}

		public Room GetRandomNeighbour() {
			return Neighbours[Random.Int(Neighbours.Count)];
		}

		public bool SetSize() {
			return SetSize(GetMinWidth(), GetMaxWidth(), GetMinHeight(), GetMaxHeight());
		}

		protected int ValidateWidth(int W) {
			return W;
		}

		protected int ValidateHeight(int H) {
			return H;
		}

		protected bool SetSize(int MinW, int MaxW, int MinH, int MaxH) {
			if (MinW < GetMinWidth() || MaxW > GetMaxWidth() || MinH < GetMinHeight() || MaxH > GetMaxHeight() || MinW > MaxW || MinH > MaxH) {
				return false;
			}

			if (Quad()) {
				var V = Math.Min(ValidateWidth(Random.Int(MinW, MaxW) - 1), ValidateHeight(Random.Int(MinH, MaxH) - 1));
				Resize(V, V);
			} else {
				Resize(ValidateWidth(Random.Int(MinW, MaxW) - 1), ValidateHeight(Random.Int(MinH, MaxH) - 1));
			}


			return true;
		}

		protected bool Quad() {
			return false;
		}

		public bool SetSizeWithLimit(int W, int H) {
			if (W < GetMinWidth() || H < GetMinHeight()) {
				return false;
			}

			SetSize();

			if (GetWidth() > W || GetHeight() > H) {
				var Ww = ValidateWidth(Math.Min(GetWidth(), W) - 1);
				var Hh = ValidateHeight(Math.Min(GetHeight(), H) - 1);

				if (Ww >= W || Hh >= H) {
					return false;
				}

				Resize(Ww, Hh);
			}

			return true;
		}

		public void ClearConnections() {
			foreach (var R in Neighbours) {
				R.Neighbours.Remove(this);
			}

			Neighbours.Clear();

			foreach (var R in Connected.Keys) {
				R.Connected.Remove(this);
			}

			Connected.Clear();
		}

		public bool CanPlaceWater(Vector2 P) {
			return Inside(P);
		}

		public List<Vector2> WaterPlaceablePoints() {
			var Points = new List<Vector2>();

			for (var I = Left + 1; I <= Right - 1; I++)
			for (var J = Top + 1; J <= Bottom - 1; J++) {
				var P = new Vector2(I, J);

				if (CanPlaceWater(P)) {
					Points.Add(P);
				}
			}

			return Points;
		}

		public bool CanPlaceGrass(Vector2 P) {
			return Inside(P);
		}

		public List<Vector2> GrassPlaceablePoints() {
			var Points = new List<Vector2>();

			for (var I = Left + 1; I <= Right - 1; I++)
			for (var J = Top + 1; J <= Bottom - 1; J++) {
				var P = new Vector2(I, J);

				if (CanPlaceGrass(P)) {
					Points.Add(P);
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
			return new Point(Left + GetWidth() / 2, Top + GetHeight() / 2);
		}

		protected Rect GetConnectionSpace() {
			var C = GetDoorCenter();

			return new Rect(C.X, C.Y, C.X, C.Y);
		}

		protected Point GetDoorCenter() {
			var DoorCenter = new Point(0, 0);

			foreach (var Door in Connected.Values) {
				DoorCenter.X += Door.X;
				DoorCenter.Y += Door.Y;
			}

			var N = Connected.Count;
			var C = new Point(DoorCenter.X / N, DoorCenter.Y / N);

			if (Random.Float() < DoorCenter.X % 1) {
				C.X++;
			}

			if (Random.Float() < DoorCenter.Y % 1) {
				C.Y++;
			}

			C.X = (int) MathUtils.Clamp(Left + 1, Right - 1, C.X);
			C.Y = (int) MathUtils.Clamp(Top + 1, Bottom - 1, C.Y);

			return C;
		}

		protected void PaintTunnel(Level Level, byte Floor) {
			PaintTunnel(Level, Floor, false);
		}

		protected void PaintTunnel(Level Level, byte Floor, bool Bold) {
			if (Connected.Count == 0) {
				Log.Error("Invalid connection room");

				return;
			}

			var C = GetConnectionSpace();

			foreach (var Door in Connected.Values) {
				var Start = new Vector2(Door.X, Door.Y);
				Vector2 Mid;
				Vector2 End;

				if ((int) Start.X == Left) {
					Start.X++;
				} else if ((int) Start.Y == Top) {
					Start.Y++;
				} else if ((int) Start.X == Right) {
					Start.X--;
				} else if ((int) Start.Y == Bottom) {
					Start.Y--;
				}

				int RightShift;
				int DownShift;

				if (Start.X < C.Left) {
					RightShift = (int) (C.Left - Start.X);
				} else if (Start.X > C.Right) {
					RightShift = (int) (C.Right - Start.X);
				} else {
					RightShift = 0;
				}

				if (Start.Y < C.Top) {
					DownShift = (int) (C.Top - Start.Y);
				} else if (Start.Y > C.Bottom) {
					DownShift = (int) (C.Bottom - Start.Y);
				} else {
					DownShift = 0;
				}

				if (Door.X == Left || Door.X == Right) {
					Mid = new Vector2(Start.X + RightShift, Start.Y);
					End = new Vector2(Mid.X, Mid.Y + DownShift);
				} else {
					Mid = new Vector2(Start.X, Start.Y + DownShift);
					End = new Vector2(Mid.X + RightShift, Mid.Y);
				}

				Painter.DrawLine(Level, Start, Mid, Floor, Bold);
				Painter.DrawLine(Level, Mid, End, Floor, Bold);
			}
		}
	}
}