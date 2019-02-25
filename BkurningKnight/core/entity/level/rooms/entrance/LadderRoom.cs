namespace BurningKnight.core.entity.level.rooms.entrance {
	public class LadderRoom : Room {
		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.ALL) {
				return 16;
			} 

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.ALL) {
				return 1;
			} 

			return 0;
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMaxWidth() {
			return 13;
		}

		public override int GetMaxHeight() {
			return 13;
		}
	}
}
