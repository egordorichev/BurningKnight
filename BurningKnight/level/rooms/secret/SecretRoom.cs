using BurningKnight.level.rooms.connection;
using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.secret {
	public class SecretRoom : RoomDef {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.FloorD);
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Secret;
			}		
		}

		public override int GetMinWidth() {
			return 8;
		}

		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 12;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 4;
			return 1;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}

		public override bool CanConnect(RoomDef R) {
			return !(R is ConnectionRoom) && base.CanConnect(R);
		}

		public override bool ShouldSpawnMobs() {
			return Random.Chance(10);
		}
	}
}