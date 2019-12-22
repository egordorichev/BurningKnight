using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class RedChest : Chest {
		protected override string GetSprite() {
			return "red_chest";
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:red_chest");
		}

		protected override bool TryOpen(Entity entity) {
			if (!entity.TryGetComponent<HealthComponent>(out var h) || h.Health < 3) {
				return false;
			}

			h.ModifyHealth(-2, this);
			return true;
		}
	}
}