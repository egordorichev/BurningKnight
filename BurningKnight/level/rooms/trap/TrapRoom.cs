namespace BurningKnight.level.rooms.trap {
	public class TrapRoom : RoomDef {
		public override void Paint(Level level) {
			// Empty to make sure that we dont paint a regular room wall here
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) {
				return 16;
			}

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) {
				return 2;
			}

			return 0;
		}
	}
}