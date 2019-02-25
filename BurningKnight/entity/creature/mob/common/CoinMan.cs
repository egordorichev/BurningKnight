using BurningKnight.entity.item;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.common {
	public class CoinMan : SupplyMan {
		public static Animation Animations = Animation.Make("actor-supply", "-coin");

		public override Animation GetAnimation() {
			return Animations;
		}

		protected override List GetDrops<Item>() {
			List<Item> Items = new List<>();

			for (var I = 0; I < Random.NewInt(8, 20); I++) Items.Add(new Gold());

			return Items;
		}
	}
}