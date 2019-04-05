using BurningKnight.entity.level.rooms;
using BurningKnight.entity.pool;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.walls {
	public class WallRegistry : Pool<WallPainter> {
		public static WallRegistry Instance = new WallRegistry();

		public WallRegistry() {
			SetupRooms();
		}

		protected virtual void SetupRooms() {
			Add(new CollumnWall(), 1f);
			Add(new CollumsWall(), 100000f);
		}

		public static void Paint(Level level, RoomDef room, WallRegistry registry = null) {
			var painter = (registry ?? Instance).Generate();
			painter.Paint(level, room, new Rect(room.Left + 1, room.Top + 1, 
				room.Left + 1 + room.GetWidth() - 2, room.Top + 1 + room.GetHeight() - 2));
		}
	}
}