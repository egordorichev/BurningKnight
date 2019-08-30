using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;

namespace BurningKnight.level.floors {
	public class FloorPainter {
		public virtual void Paint(Level level, RoomDef room, Rect inside, bool gold) {
			Painter.Fill(level, room, 1, gold ? Tile.FloorD : Tiles.RandomFloor());
		}
	}
}