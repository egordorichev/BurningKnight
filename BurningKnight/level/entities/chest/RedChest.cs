using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.player;
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
			if (entity.GetComponent<HealthComponent>().Health + entity.GetComponent<HeartsComponent>().ShieldHalfs < 3) {
				return false;
			}

			entity.GetComponent<HealthComponent>().ModifyHealth(-2, this, DamageType.Custom);
			return true;
		}
	}
}