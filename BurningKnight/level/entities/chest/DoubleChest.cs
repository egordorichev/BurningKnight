using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;

namespace BurningKnight.level.entities.chest {
	public class DoubleChest : GoldChest {
		public DoubleChest() {
			KeysRequired = 2;
		}

		protected override string GetSprite() {
			return "double_chest";
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:double_chest");
		}
	}
}