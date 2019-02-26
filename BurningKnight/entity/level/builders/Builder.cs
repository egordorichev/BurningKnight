using System;
using System.Collections.Generic;
using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.rooms.boss;
using BurningKnight.entity.level.rooms.connection;
using BurningKnight.entity.level.rooms.shop;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.level.builders {
	public class Builder {
		private const double A = 180 / Math.PI;

		protected static void FindNeighbours(List<Room> Rooms) {
			for (var I = 0; I < Rooms.Count - 1; I++) {
				for (var J = I + 1; J < Rooms.Count; J++) {
					Rooms[I].ConnectTo(Rooms[J]);
				}
			}
		}

		protected static Rect FindFreeSpace(Vector2 Start, List<Room> Collision, int MaxSize) {
			var Space = new Rect((int) (Start.X - MaxSize), (int) (Start.Y - MaxSize), (int) (Start.X + MaxSize), (int) (Start.Y + MaxSize));
			var Colliding = new List<Room>(Collision);

			do {
				for (var I = Colliding.Count - 1; I >= 0; I--) {
					var Room = Colliding[I];

					if (Room.IsEmpty() || Math.Max(Space.Left, Room.Left) >= Math.Min(Space.Right, Room.Right) || Math.Max(Space.Top, Room.Top) >= Math.Min(Space.Bottom, Room.Bottom)) {
						Colliding.RemoveAt(I);
					}
				}

				Room ClosestRoom = null;
				var ClosestDiff = int.MaxValue;
				var Inside = true;
				var CurDiff = 0;

				foreach (var CurRoom in Colliding) {
					if (Start.X <= CurRoom.Left) {
						Inside = false;
						CurDiff += CurRoom.Left - (int) Start.X;
					} else if (Start.X >= CurRoom.Right) {
						Inside = false;
						CurDiff += (int) Start.X - CurRoom.Right;
					}

					if (Start.Y <= CurRoom.Top) {
						Inside = false;
						CurDiff += CurRoom.Top - (int) Start.Y;
					} else if (Start.Y >= CurRoom.Bottom) {
						Inside = false;
						CurDiff += (int) Start.Y - CurRoom.Bottom;
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
					WDiff = Int32.MaxValue;

					if (ClosestRoom.Left >= Start.X) {
						WDiff = (Space.Right - ClosestRoom.Left) * (Space.GetHeight() + 1);
					} else if (ClosestRoom.Right <= Start.X) {
						WDiff = (ClosestRoom.Right - Space.Left) * (Space.GetHeight() + 1);
					}

					HDiff = Int32.MaxValue;

					if (ClosestRoom.Top >= Start.Y) {
						HDiff = (Space.Bottom - ClosestRoom.Top) * (Space.GetWidth() + 1);
					} else if (ClosestRoom.Bottom <= Start.Y) {
						HDiff = (ClosestRoom.Bottom - Space.Top) * (Space.GetWidth() + 1);
					}

					if (WDiff < HDiff || WDiff == HDiff && Random.Int(2) == 0) {
						if (ClosestRoom.Left >= Start.X && ClosestRoom.Left < Space.Right) {
							Space.Right = ClosestRoom.Left;
						}


						if (ClosestRoom.Right <= Start.X && ClosestRoom.Right > Space.Left) {
							Space.Left = ClosestRoom.Right;
						}
					} else {
						if (ClosestRoom.Top >= Start.Y && ClosestRoom.Top < Space.Bottom) {
							Space.Bottom = ClosestRoom.Top;
						}


						if (ClosestRoom.Bottom <= Start.Y && ClosestRoom.Bottom > Space.Top) {
							Space.Top = ClosestRoom.Bottom;
						}
					}


					Colliding.Remove(ClosestRoom);
				} else {
					Colliding.Clear();
				}
			} while (Colliding.Count != 0);

			return Space;
		}

		protected static float AngleBetweenRooms(Room From, Room To) {
			var FromCenter = new Vector2((From.Left + From.Right) / 2f, (From.Top + From.Bottom) / 2f);
			var ToCenter = new Vector2((To.Left + To.Right) / 2f, (To.Top + To.Bottom) / 2f);

			return AngleBetweenPoints(FromCenter, ToCenter);
		}

		protected static float AngleBetweenPoints(Vector2 From, Vector2 To) {
			double M = (To.Y - From.Y) / (To.X - From.X);
			var Angle = (float) (A * (Math.Atan(M) + Math.PI / 2.0));

			if (From.X > To.X) {
				Angle -= 180f;
			}

			return Angle;
		}

		protected static float PlaceRoom(List<Room> Collision, Room Prev, Room Next, float Angle) {
			Angle %= 360f;

			if (Angle < 0) {
				Angle += 360f;
			}

			var PrevCenter = new Vector2((Prev.Left + Prev.Right) / 2f, (Prev.Top + Prev.Bottom) / 2f);
			var M = Math.Tan(Angle / A + Math.PI / 2.0);
			var B = PrevCenter.Y - M * PrevCenter.X;
			Vector2 Start;
			Room.Connection Direction;

			if (Math.Abs(M) >= 1) {
				if (Angle < 90 || Angle > 270) {
					Direction = Room.Connection.TOP;
					Start = new Vector2((int) Math.Round((Prev.Top - B) / M), Prev.Top);
				} else {
					Direction = Room.Connection.BOTTOM;
					Start = new Vector2((int) Math.Round((Prev.Bottom - B) / M), Prev.Bottom);
				}
			} else {
				if (Angle < 180) {
					Direction = Room.Connection.RIGHT;
					Start = new Vector2(Prev.Right, (int) Math.Round(M * Prev.Right + B));
				} else {
					Direction = Room.Connection.LEFT;
					Start = new Vector2(Prev.Left, (int) Math.Round(M * Prev.Left + B));
				}
			}

			if (Direction == Room.Connection.TOP || Direction == Room.Connection.BOTTOM) {
				Start.X = (int) MathUtils.Clamp(Prev.Left + 1, Prev.Right - 1, Start.X);
			} else {
				Start.Y = (int) MathUtils.Clamp(Prev.Top + 1, Prev.Bottom - 1, Start.Y);
			}

			var Space = FindFreeSpace(Start, Collision, Math.Max(Next.GetMaxWidth(), Next.GetMaxHeight()));

			if (!Next.SetSizeWithLimit(Space.GetWidth() + 1, Space.GetHeight() + 1)) {
				return -1;
			}

			var TargetCenter = new Point();

			if (Direction == Room.Connection.TOP) {
				TargetCenter.Y = (int) (Prev.Top - (Next.GetHeight() - 1) / 2f);
				TargetCenter.X = (int) ((TargetCenter.Y - B) / M);
				Next.SetPos((int) Math.Round(TargetCenter.X - (Next.GetWidth() - 1) / 2f), Prev.Top - (Next.GetHeight() - 1));
			} else if (Direction == Room.Connection.BOTTOM) {
				TargetCenter.Y = (int) (Prev.Bottom + (Next.GetHeight() - 1) / 2f);
				TargetCenter.X = (int) ((TargetCenter.Y - B) / M);
				Next.SetPos((int) Math.Round(TargetCenter.X - (Next.GetWidth() - 1) / 2f), Prev.Bottom);
			} else if (Direction == Room.Connection.RIGHT) {
				TargetCenter.X = (int) (Prev.Right + (Next.GetWidth() - 1) / 2f);
				TargetCenter.Y = (int) (M * TargetCenter.X + B);
				Next.SetPos(Prev.Right, (int) Math.Round(TargetCenter.Y - (Next.GetHeight() - 1) / 2f));
			} else if (Direction == Room.Connection.LEFT) {
				TargetCenter.X = (int) (Prev.Left - (Next.GetWidth() - 1) / 2f);
				TargetCenter.Y = (int) (M * TargetCenter.X + B);
				Next.SetPos(Prev.Left - (Next.GetWidth() - 1), (int) Math.Round(TargetCenter.Y - (Next.GetHeight() - 1) / 2f));
			}

			if (Direction == Room.Connection.TOP || Direction == Room.Connection.BOTTOM) {
				if (Next.Right < Prev.Left + 2) {
					Next.Shift(Prev.Left + 2 - Next.Right, 0);
				} else if (Next.Left > Prev.Right - 2) {
					Next.Shift(Prev.Right - 2 - Next.Left, 0);
				}

				if (Next.Right > Space.Right) {
					Next.Shift(Space.Right - Next.Right, 0);
				} else if (Next.Left < Space.Left) {
					Next.Shift(Space.Left - Next.Left, 0);
				}
			} else {
				if (Next.Bottom < Prev.Top + 2) {
					Next.Shift(0, Prev.Top + 2 - Next.Bottom);
				} else if (Next.Top > Prev.Bottom - 2) {
					Next.Shift(0, Prev.Bottom - 2 - Next.Top);
				}

				if (Next.Bottom > Space.Bottom) {
					Next.Shift(0, Space.Bottom - Next.Bottom);
				} else if (Next.Top < Space.Top) {
					Next.Shift(0, Space.Top - Next.Top);
				}
			}

			if (Next.ConnectWithRoom(Prev)) {
				if (Next is ConnectionRoom || Next is BossRoom || Next is ShopRoom) {
					Next.Id = Prev.Id;
				} else {
					Next.Id = Prev.Id + 1;
				}


				return AngleBetweenRooms(Prev, Next);
			}

			return -1;
		}

		public List<Room> Build<Room>(List<Room> Init) {
			return null;
		}
	}
}