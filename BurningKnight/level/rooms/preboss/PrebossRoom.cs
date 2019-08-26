using BurningKnight.level.entities.decor;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.preboss {
	public class PrebossRoom : EntranceRoom {
		public PrebossRoom() {
			Exit = true;
		}
		
		public override void Paint(Level level) {
			Painter.Fill(level, this, Tile.WallA);

			foreach (var p in Connected) {
				var d = p.Value;

				if (p.Key is BossRoom) {
					Point a;
					Point b;

					if (d.X == Left) {
						a = new Point(Left + 4, Top + 4);
						b = new Point(Left + 4, Top + 6);
					} else if (d.X == Right) {
						a = new Point(Right - 4, Top + 4);
						b = new Point(Right - 4, Top + 6);
					} else if (d.Y == Top) {
						a = new Point(Left + 4, Top + 4);
						b = new Point(Left + 6, Top + 4);
					} else {
						a = new Point(Left + 4, Bottom - 4);
						b = new Point(Left + 6, Bottom - 4);
					}

					var ta = new Torch();
					level.Area.Add(ta);
					ta.CenterX = a.X * 16 + 8;
					ta.Bottom = a.Y * 16 + 12;
					
					var tb = new Torch();
					level.Area.Add(tb);
					tb.CenterX = b.X * 16 + 8;
					tb.Bottom = b.Y * 16 + 12;
					
				}

				Vector2 from;
				Vector2 to;

				if (d.X == Left) {
					to = new Vector2(Left + 3, Top + 5);
					from = new Vector2(d.X + 1, d.Y);
				} else if (d.X == Right) {
					to = new Vector2(Right - 3, Top + 5);
					from = new Vector2(d.X - 1, d.Y);
				} else if (d.Y == Top) {
					to = new Vector2(Left + 5, Top + 3);
					from = new Vector2(d.X, d.Y + 1);
				} else {
					to = new Vector2(Left + 5, Bottom - 3);
					from = new Vector2(d.X, d.Y - 1);
				}

				var f = Tiles.RandomFloor();

				if (d.X == Left || d.X == Right) {
					var n = new Vector2(from.X, to.Y);
					
					Painter.DrawLine(level, from, n, f);
					Painter.DrawLine(level, n, to, f);
				} else {
					var n = new Vector2(to.X, from.Y);
					
					Painter.DrawLine(level, from, n, f);
					Painter.DrawLine(level, n, to, f);
				}
			}
			
			Painter.Fill(level, this, 3, Tile.FloorB);
			Painter.Fill(level, this, 4, Tile.FloorD);
		}

		public override int GetMinWidth() {
			return 11;
		}

		public override int GetMinHeight() {
			return 11;
		}

		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMaxHeight() {
			return 12;
		}

		public override bool CanConnect(RoomDef R, Vector2 P) {
			if (R is BossRoom) {
				var x = (int) P.X;
				var y = (int) P.Y;

				if (x == Left || x == Right) {
					if (y != Top + GetHeight() / 2) {
						return false;
					}
				} else if (y == Top || y == Bottom) {
					if (x != Left + GetWidth() / 2) {
						return false;
					}
				}
			}

			return base.CanConnect(R, P);
		}

		/*public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) {
				return 3;
			}

			return 1;
		}*/
	}
}