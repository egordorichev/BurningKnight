using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.floors {
	public class HoneycombFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside, bool gold) {
			if (Rnd.Chance()) {
				Painter.Fill(level, inside, Tiles.RandomFloor());
				inside = inside.Shrink(Rnd.Int(1, 3));
			}

			var a = gold ? Tile.FloorD : Tiles.RandomFloor();
			var b = Tiles.RandomNewFloor();
			
			for (int y = inside.Top; y < inside.Bottom; y++) {
				for (int x = inside.Left; x < inside.Right; x++) {
					Painter.Set(level, x, y, (x + y * 2) % 4 == 0 ? a : b);
				}
			}
		}
	}
}