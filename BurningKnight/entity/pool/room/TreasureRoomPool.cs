using BurningKnight.entity.level.rooms.treasure;

namespace BurningKnight.entity.pool.room {
	public class TreasureRoomPool : Pool<TreasureRoom> {
		public static TreasureRoomPool Instance = new TreasureRoomPool();

		public TreasureRoomPool() {
			Add(CornerlessTreasureRoom.GetType(), 1f);
			Add(BrokeLineTreasureRoom.GetType(), 1f);
			Add(IslandTreasureRoom.GetType(), 1f);
			Add(CollumnTreasureRoom.GetType(), 1f);
			Add(MazeTreasureRoom.GetType(), 1f);
		}
	}
}