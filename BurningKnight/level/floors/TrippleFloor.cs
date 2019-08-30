using System.Linq;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.floors {
	public class TrippleFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside, bool gold) {
			var tiles = new[] {
				Tile.FloorA, Tile.FloorB, Tile.FloorC
			};

			if (gold) {
				tiles[Random.Int(3)] = Tile.FloorD;
			}

			tiles = tiles.OrderBy(x => Random.Generator.Next()).ToArray();
			
			var a = tiles[0];
			var b = tiles[1];
			var c = Random.Chance() ? a : tiles[2];
			var start = Random.Float();
			var size = Random.Int(1, 4);
			var two = Random.Chance();
						
			for (int y = inside.Top; y < inside.Bottom; y++) {
				for (int x = inside.Left; x < inside.Right; x++) {
					int val;
					
					if (two) {
						val = ((int) (x + start) % size == 0 ? 1 : ((int) (y + start)) % size == 0 ? 1 : 0) % 3;
					} else {
						val = (((int) (x + start) % size == 0 ? 1 : 0) + ((int) (y + start) % size == 0 ? 1 : 0)) % 3;
					}
					
					var tile = a;

					if (val == 1) {
						tile = b;
					} else if (val == 2) {
						tile = c;
					}
					
					Painter.Set(level, x, y, tile);
				}
			}
		}
	}
}