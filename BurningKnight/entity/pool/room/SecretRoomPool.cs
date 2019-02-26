using BurningKnight.entity.level.rooms.secret;

namespace BurningKnight.entity.pool.room {
	public class SecretRoomPool : Pool<SecretRoom> {
		public static SecretRoomPool Instance = new SecretRoomPool();

		public SecretRoomPool() {
			Add(typeof(BombRoom), 1f);
			Add(typeof(ChestRoom), 1f);
			Add(typeof(GoldSecretRoom), 1f);
			Add(typeof(HeartRoom), 1f);
			Add(typeof(MixedSecretRoom), 1f);
		}
	}
}