using BurningKnight.entity.level.rooms.special;

namespace BurningKnight.entity.pool.room {
	public class SpecialRoomPool : ClosingPool<SpecialRoom> {
		public static SpecialRoomPool Instance = new SpecialRoomPool();
	}
}