using BurningKnight.entity.level.rooms.secret;

namespace BurningKnight.entity.pool.room {
	public class SecretRoomPool : Pool<SecretRoom> {
		public static SecretRoomPool Instance = new SecretRoomPool();

		public SecretRoomPool() {
			Add(typeof(SecretRoom), 1f);
		}
	}
}