using BurningKnight.entity.level.rooms.boss;
using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.pool.room {
	public class BossRoomPool : Pool<EntranceRoom> {
		public static BossRoomPool Instance = new BossRoomPool();

		public BossRoomPool() {
			Add(SimpleBossRoom.GetType(), 1f);
			Add(CollumnsBossRoom.GetType(), 1f);
			Add(CircleBossRoom.GetType(), 1f);
		}
	}
}