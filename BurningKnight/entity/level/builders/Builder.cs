using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.rooms.boss;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.rooms.shop;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.builders {
	public class Builder {
		private const double A = 180 / Math.PI;

		protected static void FindNeighbours(List Rooms) {
			Room[] Ra = Rooms.ToArray(new Room[0]);

			for (var I = 0; I < Ra.Length - 1; I++)
			for (var J = I + 1; J < Ra.Length; J++)
				Ra[I].ConnectTo(Ra[J]);
		}

		protected static Rect FindFreeSpace(Point Start, List Collision, int MaxSize) {
			var Space = new Rect(Start.X - MaxSize, Start.Y - MaxSize, Start.X + MaxSize, Start.Y + MaxSize);
			List<Room> Colliding = new List<>(Collision);

			do {
				for (var I = Colliding.Size() - 1; I >= 0; I--) {
					Room Room = Colliding.Get(I);

					if (Room.IsEmpty() || Math.Max(Space.Left, Room.Left) >= Math.Min(Space.Right, Room.Right) || Math.Max(Space.Top, Room.Top) >= Math.Min(Space.Bottom, Room.Bottom)) Colliding.Remove(I);
				}

				Room ClosestRoom = null;
				int ClosestDiff = Integer.MAX_VALUE;
				var Inside = true;
				var CurDiff = 0;

				foreach (Room CurRoom in Colliding) {
					if (Start.X <= CurRoom.Left) {
						Inside = false;
						CurDiff += CurRoom.Left - Start.X;
					}
					else if (Start.X >= CurRoom.Right) {
						Inside = false;
						CurDiff += Start.X - CurRoom.Right;
					}

					if (Start.Y <= CurRoom.Top) {
						Inside = false;
						CurDiff += CurRoom.Top - Start.Y;
					}
					else if (Start.Y >= CurRoom.Bottom) {
						Inside = false;
						CurDiff += Start.Y - CurRoom.Bottom;
					}

					if (Inside) {
						Space.Set(new Rect((int) Start.X, (int) Start.Y, (int) Start.X, (int) Start.Y));

						return Space;
					}

					if (CurDiff < ClosestDiff) {
						ClosestDiff = CurDiff;
						ClosestRoom = CurRoom;
					}
				}

				int WDiff;
				int HDiff;

				if (ClosestRoom != null) {
					WDiff = Integer.MAX_VALUE;

					if (ClosestRoom.Left >= Start.X)
						WDiff = (Space.Right - ClosestRoom.Left) * (Space.GetHeight() + 1);
					else if (ClosestRoom.Right <= Start.X) WDiff = (ClosestRoom.Right - Space.Left) * (Space.GetHeight() + 1);

					HDiff = Integer.MAX_VALUE;

					if (ClosestRoom.Top >= Start.Y)
						HDiff = (Space.Bottom - ClosestRoom.Top) * (Space.GetWidth() + 1);
					else if (ClosestRoom.Bottom <= Start.Y) HDiff = (ClosestRoom.Bottom - Space.Top) * (Space.GetWidth() + 1);

					if (WDiff < HDiff || WDiff == HDiff && Random.NewInt(2) == 0) {
						if (ClosestRoom.Left >= Start.X && ClosestRoom.Left < Space.Right) Space.Right = ClosestRoom.Left;


						if (ClosestRoom.Right <= Start.X && ClosestRoom.Right > Space.Left) Space.Left = ClosestRoom.Right;
					}
					else {
						if (ClosestRoom.Top >= Start.Y && ClosestRoom.Top < Space.Bottom) Space.Bottom = ClosestRoom.Top;


						if (ClosestRoom.Bottom <= Start.Y && ClosestRoom.Bottom > Space.Top) Space.Top = ClosestRoom.Bottom;
					}


					Colliding.Remove(ClosestRoom);
				}
				else {
					Colliding.Clear();
				}
			} while (!Colliding.IsEmpty());

			return Space;
		}

		protected static float AngleBetweenRooms(Room From, Room To) {
			var FromCenter = new Point((From.Left + From.Right) / 2f, (From.Top + From.Bottom) / 2f);
			var ToCenter = new Point((To.Left + To.Right) / 2f, (To.Top + To.Bottom) / 2f);

			return AngleBetweenPoints(FromCenter, ToCenter);
		}

		protected static float AngleBetweenPoints(Point From, Point To) {
			double M = (To.Y - From.Y) / (To.X - From.X);
			var Angle = (float) (A * (Math.Atan(M) + Math.PI / 2.0));

			if (From.X > To.X) Angle -= 180f;

			return Angle;
		}

		protected static float PlaceRoom(List Collision, Room Prev, Room Next, float Angle) {
			Angle % 360f;

			if (Angle < 0) Angle += 360f;

			var PrevCenter = new Point((Prev.Left + Prev.Right) / 2f, (Prev.Top + Prev.Bottom) / 2f);
			double M = Math.Tan(Angle / A + Math.PI / 2.0);
			var B = PrevCenter.Y - M * PrevCenter.X;
			Point Start;
			Room.Connection Direction;

			if (Math.Abs(M) >= 1) {
				if (Angle < 90 || Angle > 270) {
					Direction = Room.Connection.TOP;
					Start = new Point((int) Math.Round((Prev.Top - B) / M), Prev.Top);
				}
				else {
					Direction = Room.Connection.BOTTOM;
					Start = new Point((int) Math.Round((Prev.Bottom - B) / M), Prev.Bottom);
				}
			}
			else {
				if (Angle < 180) {
					Direction = Room.Connection.RIGHT;
					Start = new Point(Prev.Right, (int) Math.Round(M * Prev.Right + B));
				}
				else {
					Direction = Room.Connection.LEFT;
					Start = new Point(Prev.Left, (int) Math.Round(M * Prev.Left + B));
				}
			}


			if (Direction == Room.Connection.TOP || Direction == Room.Connection.BOTTOM)
				Start.X = (int) MathUtils.Clamp(Prev.Left + 1, Prev.Right - 1, Start.X);
			else
				Start.Y = (int) MathUtils.Clamp(Prev.Top + 1, Prev.Bottom - 1, Start.Y);


			var Space = FindFreeSpace(Start, Collision, Math.Max(Next.GetMaxWidth(), Next.GetMaxHeight()));

			if (!Next.SetSizeWithLimit(Space.GetWidth() + 1, Space.GetHeight() + 1)) return -1;

			var TargetCenter = new Point();

			if (Direction == Room.Connection.TOP) {
				TargetCenter.Y = Prev.Top - (Next.GetHeight() - 1) / 2f;
				TargetCenter.X = (float) ((TargetCenter.Y - B) / M);
				Next.SetPos(Math.Round(TargetCenter.X - (Next.GetWidth() - 1) / 2f), Prev.Top - (Next.GetHeight() - 1));
			}
			else if (Direction == Room.Connection.BOTTOM) {
				TargetCenter.Y = Prev.Bottom + (Next.GetHeight() - 1) / 2f;
				TargetCenter.X = (float) ((TargetCenter.Y - B) / M);
				Next.SetPos(Math.Round(TargetCenter.X - (Next.GetWidth() - 1) / 2f), Prev.Bottom);
			}
			else if (Direction == Room.Connection.RIGHT) {
				TargetCenter.X = Prev.Right + (Next.GetWidth() - 1) / 2f;
				TargetCenter.Y = (float) (M * TargetCenter.X + B);
				Next.SetPos(Prev.Right, Math.Round(TargetCenter.Y - (Next.GetHeight() - 1) / 2f));
			}
			else if (Direction == Room.Connection.LEFT) {
				TargetCenter.X = Prev.Left - (Next.GetWidth() - 1) / 2f;
				TargetCenter.Y = (float) (M * TargetCenter.X + B);
				Next.SetPos(Prev.Left - (Next.GetWidth() - 1), Math.Round(TargetCenter.Y - (Next.GetHeight() - 1) / 2f));
			}

			if (Direction == Room.Connection.TOP || Direction == Room.Connection.BOTTOM) {
				if (Next.Right < Prev.Left + 2)
					Next.Shift(Prev.Left + 2 - Next.Right, 0);
				else if (Next.Left > Prev.Right - 2) Next.Shift(Prev.Right - 2 - Next.Left, 0);

				if (Next.Right > Space.Right)
					Next.Shift(Space.Right - Next.Right, 0);
				else if (Next.Left < Space.Left) Next.Shift(Space.Left - Next.Left, 0);
			}
			else {
				if (Next.Bottom < Prev.Top + 2)
					Next.Shift(0, Prev.Top + 2 - Next.Bottom);
				else if (Next.Top > Prev.Bottom - 2) Next.Shift(0, Prev.Bottom - 2 - Next.Top);

				if (Next.Bottom > Space.Bottom)
					Next.Shift(0, Space.Bottom - Next.Bottom);
				else if (Next.Top < Space.Top) Next.Shift(0, Space.Top - Next.Top);
			}


			if (Next.ConnectWithRoom(Prev)) {
				if (Next is ConnectionRoom || Next is BossRoom || Next is ShopRoom)
					Next.Id = Prev.Id;
				else
					Next.Id = Prev.Id + 1;


				return AngleBetweenRooms(Prev, Next);
			}

			return -1;
		}

		public List Build<Room>(List Init) {
			return null;
		}
	}
}