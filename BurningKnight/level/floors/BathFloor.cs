using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.floors {
	public class BathFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			FloorRegistry.Paint(level, room);

			var m = Random.Int(2, 4);
			
			Painter.Fill(level, room, m, Tiles.RandomFloor());
			Painter.Fill(level, room, m + 1, Tiles.RandomNewFloor());
			Painter.Fill(level, room, m + 1, Tile.Water);
		}
	}
}