using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;

namespace BurningKnight.level.entities.chest {
	public class TripleChest : GoldChest {
		public TripleChest() {
			Sprite = "triple_chest";
			KeysRequired = 3;
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:red_chest");
		}
	}
}