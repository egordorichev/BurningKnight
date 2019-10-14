using BurningKnight.level.entities.decor;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.entrance;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
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
				var g = p.Key is BossRoom;

				if (g) {
					Dot a;
					Dot b;

					if (d.X == Left) {
						a = new Dot(Left + 4, Top + 4);
						b = new Dot(Left + 4, Top + 6);
					} else if (d.X == Right) {
						a = new Dot(Right - 4, Top + 4);
						b = new Dot(Right - 4, Top + 6);
					} else if (d.Y == Top) {
						a = new Dot(Left + 4, Top + 4);
						b = new Dot(Left + 6, Top + 4);
					} else {
						a = new Dot(Left + 4, Bottom - 4);
						b = new Dot(Left + 6, Bottom - 4);
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

				Dot from;
				Dot to;

				if (d.X == Left) {
					to = new Dot(Left + 3, Top + 5);
					from = new Dot(d.X + 1, d.Y);
				} else if (d.X == Right) {
					to = new Dot(Right - 3, Top + 5);
					from = new Dot(d.X - 1, d.Y);
				} else if (d.Y == Top) {
					to = new Dot(Left + 5, Top + 3);
					from = new Dot(d.X, d.Y + 1);
				} else {
					to = new Dot(Left + 5, Bottom - 3);
					from = new Dot(d.X, d.Y - 1);
				}

				var f = g ? Tile.FloorD : Tiles.RandomFloor();

				if (d.X == Left || d.X == Right) {
					var n = new Dot(from.X, to.Y);
					
					Painter.DrawLine(level, from, n, f);
					Painter.DrawLine(level, n, to, f);
				} else {
					var n = new Dot(to.X, from.Y);
					
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
	}
}