using BurningKnight.core.entity.level.rooms.regular;

namespace BurningKnight.core.entity.pool.room {
	public class LampRoomPool : Pool<RegularRoom>  {
		public static LampRoomPool Instance = new LampRoomPool();

		public LampRoomPool() {
			Add(GardenRoom.GetType(), 1f);
			Add(SpikedRoom.GetType(), 1f);
			Add(MazeFloorRoom.GetType(), 0.3f);
			Add(RectFloorRoom.GetType(), 0.3f);
			Add(CollumnsRoom.GetType(), 1);
			Add(SmileRoom.GetType(), 0.2f);
			Add(SideChasmsRoom.GetType(), 1f);
		}
	}
}
