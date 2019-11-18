using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.floors {
	public class DiagonalFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside, bool gold) {
			if (Rnd.Chance()) {
				Painter.Fill(level, inside, Tiles.RandomFloor());
				inside = inside.Shrink(Rnd.Int(1, 3));
			}
			
			var a = gold ? Tile.FloorD : Tiles.RandomFloor();
			var b = Tiles.RandomNewFloor();
			var start = Rnd.Float();
			var sign = Rnd.Chance() ? 1 : -1;
			var size = (float) Rnd.Int(1, 4); // Cast to float for division later on
			
			for (int y = inside.Top; y < inside.Bottom; y++) {
				for (int x = inside.Left; x < inside.Right; x++) {
					Painter.Set(level, x, y, (int) (x / size + sign * (y / size) + start) % 2 == 0 ? a : b);
				}
			}
		}
	}
}