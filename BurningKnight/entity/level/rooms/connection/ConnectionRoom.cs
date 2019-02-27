using BurningKnight.entity.pool.room;
using BurningKnight.state;

namespace BurningKnight.entity.level.rooms.connection {
	public class ConnectionRoomDef : RoomDef {
		public static ConnectionRoomDef Create() {
			if (Run.Depth == -1 || Run.Depth == 4) {
				return new TunnelRoomDef();
			}

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
			if (Side == Connection.All) return 16;

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 2;

			return 0;
		}
	}
}