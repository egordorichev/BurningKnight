using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.pool.room {
	public class EntranceRoomPool : Pool<EntranceRoom> {
		public static EntranceRoomPool Instance = new EntranceRoomPool();

		public EntranceRoomPool() {
			Add(typeof(EntranceRoom), 1f);
			Add(typeof(CircleEntranceRoom), 1f);
			Add(typeof(LineEntranceRoom), 1f);
			Add(typeof(LineCircleRoom), 1f);
		}
	}
}