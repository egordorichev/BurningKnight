using BurningKnight.entity.pool.room;

namespace BurningKnight.entity.level.rooms.connection {
	public class ConnectionRoom : Room {
		public static ConnectionRoom Create() {
			if (Dungeon.Depth == -1 || Dungeon.Depth == 4) return new TunnelRoom();

			return ConnectionRoomPool.Instance.Generate();
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
			if (Side == Connection.ALL) return 16;

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.ALL) return 2;

			return 0;
		}
	}
}