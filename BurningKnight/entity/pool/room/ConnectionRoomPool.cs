using BurningKnight.entity.level.rooms.connection;

namespace BurningKnight.entity.pool.room {
	public class ConnectionRoomPool : Pool<ConnectionRoom> {
		public static ConnectionRoomPool Instance = new ConnectionRoomPool();

		public ConnectionRoomPool() {
			this.Add(TunnelRoom.GetType(), 5);
			this.Add(ChasmTunnelRoom.GetType(), 3);
			this.Add(RingConnectionRoom.GetType(), 3);
			this.Add(SpikedTunnelRoom.GetType(), 2);
			this.Add(BigRingConnectionRoom.GetType(), 2);
			this.Add(EmptyConnectionRoom.GetType(), 3);
			this.Add(ChasmConnectionRoom.GetType(), 2);
		}
	}
}