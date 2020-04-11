using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;

namespace BurningKnight.level.entities.chest {
	public class TripleChest : GoldChest {
		public override string GetPool() {
			return "bk:triple_chest";
		}

		public override string GetSprite() {
			return "triple_chest";
		}
	}
}