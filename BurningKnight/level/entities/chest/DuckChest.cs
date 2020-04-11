using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class DuckChest : Chest {
		public override string GetSprite() {
			return "duck_chest";
		}

		public override string GetPool() {
			return "bk:duck_chest";
		}

		protected override bool TryOpen(Entity entity) {
			return true;
		}
	}
}