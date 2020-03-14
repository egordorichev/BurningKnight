using System;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class CornerWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var t = Tiles.RandomFillWall();
			var hw = room.GetWidth() / 2;
			var hh = room.GetHeight() / 2;
			var before = Rnd.Chance();
			var frame = Rnd.Chance();

			if (before && frame) {
				Painter.Rect(level, room, 1, Tile.Chasm);
			}
			
			switch (Rnd.Int(4)) {
				case 0: {
					Painter.Fill(level, room.Left + 1, room.Top + 1, hw, hh, t);
					break;
				}

				case 1: {
					Painter.Fill(level, room.Right - hw, room.Top + 1, hw, hh, t);
					break;
				}

				case 2: {
					Painter.Fill(level, room.Left + 1, room.Bottom - hh, hw, hh, t);
					break;
				}

				default: {
					Painter.Fill(level, room.Right - hw, room.Bottom - hh, hw, hh, t);
					break;
				}
			}

			if (Rnd.Chance()) {
				var s = Math.Min(inside.GetWidth(), inside.GetHeight());
				var c = room.GetTileCenter();
				var sz = s / 3;
				inside = new Rect(c.X - sz / 2, c.Y - sz / 2).Resize(sz, sz);
				
				Painter.Fill(level, inside, Tiles.Pick(Tile.Chasm, Tiles.RandomFillWall()));

				if (Rnd.Chance()) {
					inside = inside.Shrink(Rnd.Int(1, 3));
					Painter.Fill(level, inside, Tiles.Pick(Tile.Chasm, Tiles.RandomFillWall()));
				}
			}
			
			if (!before && frame) {
				Painter.Rect(level, room, 1, Tile.Chasm);
			}
			
			room.PaintTunnel(level, Tiles.RandomFloor(), room.GetCenterRect(), Rnd.Chance(30));
		}
	}
}