using System;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class SplitWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var m = Rnd.Chance();
			
			if (m) {
				Painter.DrawLine(level, new Dot(room.Left + 1, room.Top + room.GetHeight() / 2), new Dot(room.Right - 1, room.Top + room.GetHeight() / 2), Tile.Chasm);

				if (Rnd.Chance()) {
					Painter.DrawLine(level, new Dot(room.Left + 1, room.Top + room.GetHeight() / 2 + 1), new Dot(room.Right - 1, room.Top + room.GetHeight() / 2 + 1), Tile.Chasm);
				}
			}

			if (!m || Rnd.Chance()) {
				Painter.DrawLine(level, new Dot(room.Left + room.GetWidth() / 2, room.Top + 1), new Dot(room.Left + room.GetWidth() / 2, room.Bottom - 1), Tile.Chasm);

				if (Rnd.Chance()) {
					Painter.DrawLine(level, new Dot(room.Left + room.GetWidth() / 2 + 1, room.Top + 1), new Dot(room.Left + room.GetWidth() / 2 + 1, room.Bottom - 1), Tile.Chasm);
				}
			}

			if (Rnd.Chance()) {
				var s = Math.Min(inside.GetWidth(), inside.GetHeight());
				var c = room.GetTileCenter();
				var sz = s / 2;
				inside = new Rect(c.X - sz / 2, c.Y - sz / 2).Resize(sz, sz);
				
				Painter.Fill(level, inside, Tiles.RandomFillWall());

				if (Rnd.Chance()) {
					inside = inside.Shrink(Rnd.Int(1, 3));
					Painter.Fill(level, inside, Tiles.Pick(Tile.Chasm, Tiles.RandomFillWall()));
				}
			}
		}
	}
}