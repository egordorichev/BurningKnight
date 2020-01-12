using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.floors {
	public class FancyFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside, bool gold) {
			if (Rnd.Chance()) {
				Painter.Fill(level, inside, Tiles.RandomFloor());
				inside = inside.Shrink(Rnd.Int(1, 3));
			}

			var a = gold ? Tile.FloorD : Tiles.RandomFloor();
			var b = Tiles.RandomNewFloor();
			var c = Tiles.RandomNewFloor();
			
			for (int y = inside.Top; y < inside.Bottom; y++) {
				for (int x = inside.Left; x < inside.Right; x++) {
					var z = (x + y * 3) % 5;
					Painter.Set(level, x, y, z == 0 ? a : ((z < 3) ^ (y % 2 == 0)) ? b : c);
				}
			}
		}
	}
}