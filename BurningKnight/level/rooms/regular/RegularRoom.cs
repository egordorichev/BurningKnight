using BurningKnight.state;

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

		protected bool SmallerRooms() {
			return Run.Depth <= 2;
		}
		
		public override int GetMinWidth() {
			return SmallerRooms() ? 8 : 12;
		}

		public override int GetMinHeight() {
			return SmallerRooms() ? 8 : 10;
		}

		public override int GetMaxWidth() {
			return SmallerRooms() ? 16 : 24;
		}

		public override int GetMaxHeight() {
			return SmallerRooms() ? 12 : 14;
		}
	}
}