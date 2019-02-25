using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.npc {
	public class OrangeShopkeeper : Shopkeeper {
		private static Animation Animations = Animation.Make("actor-trader", "-orange");

		public override Animation GetAnimation() {
			return Animations;
		}
	}
}
