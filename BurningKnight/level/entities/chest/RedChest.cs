using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class RedChest : Chest {
		public override string GetSprite() {
			return "red_chest";
		}

		public override string GetPool() {
			return "bk:red_chest";
		}

		protected override bool TryOpen(Entity entity) {
			if (entity.GetComponent<HealthComponent>().Health + entity.GetComponent<HeartsComponent>().Total < 3) {
				return false;
			}

			entity.GetComponent<HealthComponent>().ModifyHealth(-2, this, DamageType.Custom);
			return true;
		}
	}
}