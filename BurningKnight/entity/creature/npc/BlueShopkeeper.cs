using BurningKnight.util;

namespace BurningKnight.entity.creature.npc {
	public class BlueShopkeeper : Shopkeeper {
		private static Animation Animations = Animation.Make("actor-trader", "-blue");

		public BlueShopkeeper() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 40;
			}
		}

		public override Animation GetAnimation() {
			return Animations;
		}
	}
}