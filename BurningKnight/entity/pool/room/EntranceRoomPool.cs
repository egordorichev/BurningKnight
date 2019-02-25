using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.pool.room {
	public class EntranceRoomPool : Pool<EntranceRoom> {
		public static EntranceRoomPool Instance = new EntranceRoomPool();

		public EntranceRoomPool() {
			Add(EntranceRoom.GetType(), 1f);
			Add(CircleEntranceRoom.GetType(), 1f);
			Add(LineEntranceRoom.GetType(), 1f);
			Add(LineCircleRoom.GetType(), 1f);
		}
	}
}