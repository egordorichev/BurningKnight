using BurningKnight.entity.component;
using BurningKnight.util;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class HealChoiceStand : SingleChoiceStand {
		protected override string GetSprite() {
			return "health_stand";
		}

		protected override bool CanInteract(Entity e) {
			return true;
		}

		protected override bool Interact(Entity entity) {
			var h = entity.GetComponent<HealthComponent>();
			h.ModifyHealth(h.MaxHealth, this);

			RemoveStands();

			Done = true;
			AnimationUtil.Poof(Center);
			
			return true;
		}
	}
}