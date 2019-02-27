using BurningKnight.entity.level.rooms.treasure;

namespace BurningKnight.entity.pool.room {
	public class TreasureRoomPool : Pool<TreasureRoom> {
		public static TreasureRoomPool Instance = new TreasureRoomPool();

		public TreasureRoomPool() {
			Add(typeof(TreasureRoom), 1f);
		}
	}
}