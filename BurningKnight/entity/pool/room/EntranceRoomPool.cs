using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.pool.room {
	public class EntranceRoomPool : Pool<EntranceRoom> {
		public static EntranceRoomPool Instance = new EntranceRoomPool();

		public EntranceRoomPool() {
			Add(typeof(EntranceRoom), 1f);
		}
	}
}