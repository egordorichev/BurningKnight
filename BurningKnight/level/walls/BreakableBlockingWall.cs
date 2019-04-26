using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class BreakableBlockingWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var mx = 0;
			var my = 0;

			var a = Random.Int(inside.GetWidth() / 4, inside.GetWidth() / 3);

			if (Random.Chance()) {
				mx = a;
			} else {
				my = a;
			}
			
			inside = inside.Shift(mx, my).Shrink(mx * 2, my * 2);
			Painter.Fill(level, inside, 0, Tile.Planks);
		}
	}
}