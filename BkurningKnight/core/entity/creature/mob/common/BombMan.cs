using BurningKnight.core.entity.item;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.common {
	public class BombMan : SupplyMan {
		public static Animation Animations = Animation.Make("actor-supply", "-bomb");

		public override Animation GetAnimation() {
			return Animations;
		}

		protected override List GetDrops<Item> () {
			List<Item> Items = new List<>();
			Items.Add(new Bomb());

			return Items;
		}
	}
}
