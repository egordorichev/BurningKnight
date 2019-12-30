using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class DuckChest : Chest {
		protected override string GetSprite() {
			return "duck_chest";
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:duck_chest");
		}

		protected override bool TryOpen(Entity entity) {
			return true;
		}
	}
}