using BurningKnight.level.rooms.entrance;

namespace BurningKnight.level.rooms.boss {
	public class BossRoom : EntranceRoom {
		public override int GetMinWidth() {
			return 18 + 5;
		}

		public override int GetMinHeight() {
			return 18 + 5;
		}

		public override int GetMaxWidth() {
			return 36;
		}

		public override int GetMaxHeight() {
			return 36;
		}

		public override int GetMaxConnections(RoomDef.Connection Side) {
			return 1;
		}

		public override int GetMinConnections(RoomDef.Connection Side) {
			if (Side == Connection.All) return 1;

			return 0;
		}
	}
}