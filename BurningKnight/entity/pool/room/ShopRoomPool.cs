using BurningKnight.entity.level.rooms.shop;
using BurningKnight.entity.level.rooms.special;

namespace BurningKnight.entity.pool.room {
	public class ShopRoomPool : Pool<ShopRoom> {
		public static ShopRoomPool Instance = new ShopRoomPool();

		public ShopRoomPool() {
			Add(typeof(ShopRoom), 1);
		}
	}
}