using BurningKnight.entity.level.rooms.boss;
using BurningKnight.entity.level.rooms.entrance;

namespace BurningKnight.entity.pool.room {
	public class BossRoomPool : Pool<BossRoom> {
		public static BossRoomPool Instance = new BossRoomPool();

		public BossRoomPool() {
			
		}
	}
}