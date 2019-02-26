using BurningKnight.entity.level.rooms.connection;

namespace BurningKnight.entity.pool.room {
	public class ConnectionRoomPool : Pool<ConnectionRoom> {
		public static ConnectionRoomPool Instance = new ConnectionRoomPool();

		public ConnectionRoomPool() {
			Add(typeof(TunnelRoom), 5);
			Add(typeof(ChasmTunnelRoom), 3);
			Add(typeof(RingConnectionRoom), 3);
			Add(typeof(SpikedTunnelRoom), 2);
			Add(typeof(BigRingConnectionRoom), 2);
			Add(typeof(EmptyConnectionRoom), 3);
			Add(typeof(ChasmConnectionRoom), 2);
		}
	}
}