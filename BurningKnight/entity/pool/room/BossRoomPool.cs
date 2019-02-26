using BurningKnight.entity.level.rooms.boss;
using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.pool.room {
	public class BossRoomPool : Pool<EntranceRoom> {
		public static BossRoomPool Instance = new BossRoomPool();

		public BossRoomPool() {
			Add(typeof(SimpleBossRoom), 1f);
			Add(typeof(CollumnsBossRoom), 1f);
			Add(typeof(CircleBossRoom), 1f);
		}
	}
}