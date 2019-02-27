using BurningKnight.entity.level.rooms.shop;
using BurningKnight.entity.level.rooms.special;

namespace BurningKnight.entity.pool.room {
	public class ShopRoomPool : Pool<SpecialRoomDef> {
		public static ShopRoomPool Instance = new ShopRoomPool();

		public ShopRoomPool() {
			Add(typeof(ClassicShopRoomDef), 1);
			Add(typeof(QuadShopRoomDef), 1);
			Add(typeof(GoldShopRoomDef), 1);
		}
	}
}