using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;

namespace BurningKnight.level.entities.chest {
	public class DoubleChest : GoldChest {
		public DoubleChest() {
			KeysRequired = 2;
		}

		public override string GetSprite() {
			return "double_chest";
		}

		public override string GetPool() {
			return "bk:double_chest";
		}
	}
}