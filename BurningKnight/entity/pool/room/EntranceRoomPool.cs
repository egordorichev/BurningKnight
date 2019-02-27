using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.pool.room {
	public class EntranceRoomPool : Pool<EntranceRoomDef> {
		public static EntranceRoomPool Instance = new EntranceRoomPool();

		public EntranceRoomPool() {
			Add(typeof(EntranceRoomDef), 1f);
			Add(typeof(CircleEntranceRoomDef), 1f);
			Add(typeof(LineEntranceRoomDef), 1f);
			Add(typeof(LineCircleRoomDef), 1f);
		}
	}
}