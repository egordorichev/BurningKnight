using BurningKnight.entity.level.rooms;
using BurningKnight.entity.level.tile;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.floors {
	public class FloorPainter {
		public virtual void Paint(Level level, RoomDef room, Rect inside) {
			Painter.Fill(level, inside, Tiles.RandomFloor());
		}
	}
}