using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.accessory.hat;

namespace BurningKnight.core.entity.pool.item {
	public class ShopHatPool : Pool<Item>  {
		public static ShopHatPool Instance = new ShopHatPool();

		public ShopHatPool() {
			Add(RaveHat.GetType(), 1f);
			Add(DunceHat.GetType(), 1f);
			Add(GoldHat.GetType(), 0.2f);
			Add(RubyHat.GetType(), 0.1f);
			Add(CoboiHat.GetType(), 0.8f);
			Add(FungiHat.GetType(), 1f);
			Add(MoaiHat.GetType(), 0.5f);
			Add(ShroomHat.GetType(), 0.3f);
			Add(SkullHat.GetType(), 0.2f);
			Add(UshankaHat.GetType(), 0.3f);
			Add(ValkyreHat.GetType(), 1f);
			Add(VikingHat.GetType(), 1f);
		}
	}
}
