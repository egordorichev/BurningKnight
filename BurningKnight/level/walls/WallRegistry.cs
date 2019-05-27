using BurningKnight.entity.pool;
using BurningKnight.level.rooms;
using BurningKnight.util.geometry;

namespace BurningKnight.level.walls {
	public class WallRegistry : Pool<WallPainter> {
		public static WallRegistry Instance = new WallRegistry();

		public WallRegistry() {
			SetupRooms();
		}

		protected virtual void SetupRooms() {
			Add(new WallPainter(), 1f);

			Add(new CollumnWall(), 1f);
			Add(new CollumsWall(), 1f);
			Add(new EllipseWalls(), 1f);
			Add(new TempleWalls(), 1f);
			
			Add(new BreakableCageWall(), 0.3f);
			Add(new BreakableBlockingWall(), 0.3f);
		}

		public static void Paint(Level level, RoomDef room, WallRegistry registry = null, int i = -1) {
			var painter = i == -1 ? (registry ?? Instance).Generate() : (registry ?? Instance).Get(i);
			painter.Paint(level, room, new Rect(room.Left + 1, room.Top + 1, 
				room.Left + 1 + room.GetWidth() - 2, room.Top + 1 + room.GetHeight() - 2));
		}
	}
}