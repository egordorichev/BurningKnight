namespace BurningKnight.entity.level.rooms.special {
	public class SpecialRoom : RoomDef {
		public override int GetMinWidth() {
			return 8;
		}

		public override int GetMaxWidth() {
			return 14;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 14;
		}

		public override int GetMaxConnections(Connection Side) {
			return 1;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;

			return 0;
		}
	}
}