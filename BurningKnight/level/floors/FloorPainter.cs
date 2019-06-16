using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;

namespace BurningKnight.level.floors {
	public class FloorPainter {
		public virtual void Paint(Level level, RoomDef room, Rect inside) {
			Painter.Fill(level, room, 1, Tiles.RandomFloor());
		}
	}
}