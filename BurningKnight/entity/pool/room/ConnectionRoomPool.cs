using BurningKnight.entity.level.rooms.connection;

namespace BurningKnight.entity.pool.room {
	public class ConnectionRoomPool : Pool<ConnectionRoomDef> {
		public static ConnectionRoomPool Instance = new ConnectionRoomPool();

		public ConnectionRoomPool() {
			Add(typeof(TunnelRoomDef), 5);
			Add(typeof(ChasmTunnelRoomDef), 3);
			Add(typeof(RingConnectionRoomDef), 3);
			Add(typeof(SpikedTunnelRoomDef), 2);
			Add(typeof(BigRingConnectionRoomDef), 2);
			Add(typeof(EmptyConnectionRoomDef), 3);
			Add(typeof(ChasmConnectionRoomDef), 2);
		}
	}
}