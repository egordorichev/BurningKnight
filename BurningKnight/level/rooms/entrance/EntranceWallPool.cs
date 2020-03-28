using BurningKnight.level.walls;

namespace BurningKnight.level.rooms.entrance {
	public class EntranceWallPool : WallRegistry {
		public static EntranceWallPool Instance = new EntranceWallPool();
		
		protected override void SetupRooms() {
			Add(new WallPainter(), 1f);
			
			Add(new EllipseWalls(), 1f);
			Add(new PatchWall(), 1);
			Add(new SegmentedWall(), 1f);
			Add(new RuinsWall(), 1f);
			Add(new CollumsWall(), 1f);
			Add(new PlatformWall(), 1f);
			Add(new TempleWalls(), 1f);
			Add(new CornerWall(), 1f);
		}
	}
}