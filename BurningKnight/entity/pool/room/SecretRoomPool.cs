using BurningKnight.entity.level.rooms.secret;

namespace BurningKnight.entity.pool.room {
	public class SecretRoomPool : Pool<SecretRoomDef> {
		public static SecretRoomPool Instance = new SecretRoomPool();

		public SecretRoomPool() {
			Add(typeof(BombRoomDef), 1f);
			Add(typeof(ChestRoomDef), 1f);
			Add(typeof(GoldSecretRoomDef), 1f);
			Add(typeof(HeartRoomDef), 1f);
			Add(typeof(MixedSecretRoomDef), 1f);
		}
	}
}