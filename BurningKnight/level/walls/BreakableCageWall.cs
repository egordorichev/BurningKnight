using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class BreakableCageWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var bold = Random.Chance();
			
			if (Random.Chance()) {
				Painter.Ellipse(level, inside, Random.Int(1, 4), Tile.Planks, bold);
			} else {
				Painter.Rect(level, inside, Random.Int(1, 4), Tile.Planks, bold);
			}
		}
	}
}