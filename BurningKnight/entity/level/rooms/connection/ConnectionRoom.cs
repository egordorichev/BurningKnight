using Lens.util.math;

namespace BurningKnight.entity.level.rooms.connection {
	public class ConnectionRoom : RoomDef {
		public override void Paint(Level level) {
			if (Random.Chance()) {
				return;
			}
			
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Tunnel;
			}
		}

		public override int GetMinWidth() {
			return 3;
		}

		public override int GetMinHeight() {
			return 3;
		}

		public override int GetMaxWidth() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 10;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 16;

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 2;

			return 0;
		}
	}
}