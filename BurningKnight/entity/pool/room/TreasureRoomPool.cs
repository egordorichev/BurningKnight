using BurningKnight.entity.level.rooms.treasure;

namespace BurningKnight.entity.pool.room {
	public class TreasureRoomPool : Pool<TreasureRoomDef> {
		public static TreasureRoomPool Instance = new TreasureRoomPool();

		public TreasureRoomPool() {
			Add(typeof(CornerlessTreasureRoomDef), 1f);
			Add(typeof(BrokeLineTreasureRoomDef), 1f);
			Add(typeof(IslandTreasureRoomDef), 1f);
			Add(typeof(CollumnTreasureRoomDef), 1f);
			Add(typeof(MazeTreasureRoomDef), 1f);
		}
	}
}