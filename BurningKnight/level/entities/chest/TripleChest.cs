using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;

namespace BurningKnight.level.entities.chest {
	public class TripleChest : GoldChest {
		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:triple_chest");
		}
		
		protected override string GetSprite() {
			return "triple_chest";
		}
	}
}