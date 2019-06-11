using BurningKnight.level.rooms.entrance;

namespace BurningKnight.level.rooms.boss {
	public class BossRoom : ExitRoom {
		public override int GetMinWidth() {
			return 18 + 5;
		}

		public override int GetMinHeight() {
			return 18 + 5;
		}

		public override int GetMaxWidth() {
			return 30;
		}

		public override int GetMaxHeight() {
			return 30;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 2;
			return 1;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}
	}
}