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
			Painter.Fill(level, this, 1, Tiles.RandomFloor());
			Painter.Fill(level, this, 2, Tile.FloorD);

			foreach (var p in Connected) {
				if (p.Key is BossRoom) {
					var d = p.Value;
					Point a;
					Point b;

					if (d.X == Left) {
						a = new Point(Left + 2, Top + 2);
						b = new Point(Left + 2, Top + 4);
					} else if (d.X == Right) {
						a = new Point(Right - 2, Top + 2);
						b = new Point(Right - 2, Top + 4);
					} else if (d.Y == Top) {
						a = new Point(Left + 2, Top + 2);
						b = new Point(Left + 4, Top + 2);
					} else {
						a = new Point(Left + 2, Bottom - 2);
						b = new Point(Left + 4, Bottom - 2);
					}

					var ta = new Torch();
					level.Area.Add(ta);
					ta.CenterX = a.X * 16 + 8;
					ta.Bottom = a.Y * 16 + 12;
					
					var tb = new Torch();
					level.Area.Add(tb);
					tb.CenterX = b.X * 16 + 8;
					tb.Bottom = b.Y * 16 + 12;
					
					break;
				}
			}
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 8;
		}

		public override bool CanConnect(Vector2 P) {
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
			
			return base.CanConnect(P);
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) {
				return 3;
			}

			return 1;
		}
	}
}