using BurningKnight.entity.level.rooms.connection;

namespace BurningKnight.entity.pool.room {
	public class ConnectionRoomPool : Pool<ConnectionRoomDef> {
		public static ConnectionRoomPool Instance = new ConnectionRoomPool();

		public ConnectionRoomPool() {
			Add(typeof(TunnelRoomDef), 5);
		}
	}
}