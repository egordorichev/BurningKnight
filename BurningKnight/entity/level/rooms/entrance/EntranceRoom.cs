using BurningKnight.entity.level.walls;

namespace BurningKnight.entity.level.rooms.entrance {
	public class EntranceRoom : RoomDef {
		public override bool CanConnect(RoomDef R) {
			return base.CanConnect(R) && !(R is EntranceRoom);
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 16;
			return 4;
		}

		public override void Paint(Level level) {
			WallRegistry.Paint(level, this, EntranceWallPool.Instance);
			
			Painter.Prefab(level, "test", Left + 1, Top + 1);
			
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Regular;
			}
		}
	}
}