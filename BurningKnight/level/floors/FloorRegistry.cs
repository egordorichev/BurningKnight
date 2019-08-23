using BurningKnight.entity.pool;
using BurningKnight.level.rooms;
using BurningKnight.util.geometry;

namespace BurningKnight.level.floors {
	public class FloorRegistry : Pool<FloorPainter> {
		public static FloorRegistry Instance = new FloorRegistry();

		public FloorRegistry() {
			Add(new FloorPainter(), 1f);
			Add(new ChessFloor(), 1f);
			Add(new DiagonalFloor(), 1f);
			Add(new GeometryFloor(), 1f);
			Add(new MazeFloor(), 1f);
			Add(new TrippleFloor(), 1f);
			Add(new LineFloor(), 1f);
			Add(new BathFloor(), 1f);
			Add(new PatchFloor(), 1f);
		}

		public static void Paint(Level level, RoomDef room, int i = -1) {
			var painter = i == -1 ? Instance.Generate() : Instance.Get(i);
			painter.Paint(level, room, new Rect(room.Left + 1, room.Top + 1, 
				room.Left + 1 + room.GetWidth() - 2, room.Top + 1 + room.GetHeight() - 2));
		}
	}
}