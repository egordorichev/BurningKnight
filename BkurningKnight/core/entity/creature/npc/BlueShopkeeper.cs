using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.npc {
	public class BlueShopkeeper : Shopkeeper {
		protected void _Init() {
			{
				HpMax = 40;
			}
		}

		private static Animation Animations = Animation.Make("actor-trader", "-blue");

		public override Animation GetAnimation() {
			return Animations;
		}

		public BlueShopkeeper() {
			_Init();
		}
	}
}
