namespace BurningKnight.level.rooms.regular {
	public class RegularRoom : RoomDef {
		public override void SetupDoors(Level level) {
			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Enemy;
			}
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) {
				return 16;
			}

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) {
				return 1;
			}

			return 0;
		}

		public override bool ShouldSpawnMobs() {
			return true;
		}
		
		public override int GetMinWidth() {
			return 10 + 4;
		}

		public override int GetMinHeight() {
			return 8 + 4;
		}

		public override int GetMaxWidth() {
			return 18 + 8;
		}

		public override int GetMaxHeight() {
			return 12 + 8;
		}
	}
}