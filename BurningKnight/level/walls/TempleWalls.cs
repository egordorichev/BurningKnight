using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class TempleWalls : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var m = 2;
			var f = Tiles.RandomFloorOrSpike();
			var bold = Random.Chance(30);

			inside = inside.Shrink(m);
			
			Painter.Fill(level, inside, Tiles.Pick(Tile.Chasm, Tile.WallA, Tile.Planks));
			Painter.Fill(level, inside, 1, f);

			Painter.Set(level, inside.Left, inside.Top + inside.GetHeight() / 2, f);
			Painter.Set(level, inside.Right - 1, inside.Top + inside.GetHeight() / 2, f);
			
			Painter.Set(level, inside.Left + inside.GetWidth() / 2, inside.Top, f);
			Painter.Set(level, inside.Left + inside.GetWidth() / 2, inside.Bottom - 1, f);

			if (bold) {
				Painter.Set(level, inside.Left, inside.Top + inside.GetHeight() / 2 - 1, f);
				Painter.Set(level, inside.Right - 1, inside.Top + inside.GetHeight() / 2 - 1, f);
			
				Painter.Set(level, inside.Left + inside.GetWidth() / 2 - 1, inside.Top, f);
				Painter.Set(level, inside.Left + inside.GetWidth() / 2 - 1, inside.Bottom - 1, f);
				
				Painter.Set(level, inside.Left, inside.Top + inside.GetHeight() / 2 + 1, f);
				Painter.Set(level, inside.Right - 1, inside.Top + inside.GetHeight() / 2 + 1, f);
			
				Painter.Set(level, inside.Left + inside.GetWidth() / 2 + 1, inside.Top, f);
				Painter.Set(level, inside.Left + inside.GetWidth() / 2 + 1, inside.Bottom - 1, f);
			} else if (Random.Chance()) {
				f = Tiles.RandomFloor();
				
				Painter.Set(level, inside.Left, inside.Top, f);
				Painter.Set(level, inside.Right - 1, inside.Top, f);
			
				Painter.Set(level, inside.Left, inside.Bottom - 1, f);
				Painter.Set(level, inside.Right - 1, inside.Bottom - 1, f);
			}
		}
	}
}