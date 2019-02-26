using BurningKnight.entity.level.rooms.treasure;

namespace BurningKnight.entity.pool.room {
	public class TreasureRoomPool : Pool<TreasureRoom> {
		public static TreasureRoomPool Instance = new TreasureRoomPool();

		public TreasureRoomPool() {
			Add(typeof(CornerlessTreasureRoom), 1f);
			Add(typeof(BrokeLineTreasureRoom), 1f);
			Add(typeof(IslandTreasureRoom), 1f);
			Add(typeof(CollumnTreasureRoom), 1f);
			Add(typeof(MazeTreasureRoom), 1f);
		}
	}
}