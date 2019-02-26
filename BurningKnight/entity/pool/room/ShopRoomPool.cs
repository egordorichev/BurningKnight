using BurningKnight.entity.level.rooms.shop;
using BurningKnight.entity.level.rooms.special;

namespace BurningKnight.entity.pool.room {
	public class ShopRoomPool : Pool<SpecialRoom> {
		public static ShopRoomPool Instance = new ShopRoomPool();

		public ShopRoomPool() {
			Add(typeof(ClassicShopRoom), 1);
			Add(typeof(QuadShopRoom), 1);
			Add(typeof(GoldShopRoom), 1);
		}
	}
}