using BurningKnight.core.entity.level.rooms.secret;

namespace BurningKnight.core.entity.pool.room {
	public class SecretRoomPool : Pool<SecretRoom>  {
		public static SecretRoomPool Instance = new SecretRoomPool();

		public SecretRoomPool() {
			Add(BombRoom.GetType(), 1f);
			Add(ChestRoom.GetType(), 1f);
			Add(GoldSecretRoom.GetType(), 1f);
			Add(HeartRoom.GetType(), 1f);
			Add(MixedSecretRoom.GetType(), 1f);
		}
	}
}
