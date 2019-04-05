using BurningKnight.entity.level.rooms;
using BurningKnight.entity.pool;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.floors {
	public class FloorRegistry : Pool<FloorPainter> {
		public static FloorRegistry Instance = new FloorRegistry();

		public FloorRegistry() {
			Add(new FloorPainter(), 1f);
			Add(new ChessFloor(), 1f);
			Add(new DiagonalFloor(), 1f);
			Add(new GeometryFloor(), 1000000000000f);
		}

		public static void Paint(Level level, RoomDef room) {
			var painter = Instance.Generate();
			painter.Paint(level, room, new Rect(room.Left + 1, room.Top + 1, 
				room.Left + 1 + room.GetWidth() - 2, room.Top + 1 + room.GetHeight() - 2));
		}
	}
}