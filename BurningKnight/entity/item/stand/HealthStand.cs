using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class HealthStand : ItemStand {
		protected override string GetSprite() {
			return "health_stand";
		}

		protected override bool CanInteract(Entity e) {
			if (Item == null) {
				return false;
			}
			
			return base.CanInteract(e);
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);

			var h = who.GetComponent<HealthComponent>();
			h.ModifyHealth(h.MaxHealth, this);
			
			TextParticle.Add(who, "HP", h.MaxHealth, true);
		}
	}
}