using BurningKnight.core.entity.level.rooms.special;

namespace BurningKnight.core.entity.pool.room {
	public class SpecialRoomPool : ClosingPool<SpecialRoom>  {
		public static SpecialRoomPool Instance = new SpecialRoomPool();

		public SpecialRoomPool() {

		}
	}
}
