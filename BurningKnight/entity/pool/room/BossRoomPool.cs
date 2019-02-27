using BurningKnight.entity.level.rooms.boss;
using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.pool.room {
	public class BossRoomPool : Pool<EntranceRoomDef> {
		public static BossRoomPool Instance = new BossRoomPool();

		public BossRoomPool() {
			Add(typeof(SimpleBossRoomDef), 1f);
			Add(typeof(CollumnsBossRoomDef), 1f);
			Add(typeof(CircleBossRoomDef), 1f);
		}
	}
}