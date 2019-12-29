using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ReplaceHeartsWithShieldsUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			var h = entity.GetComponent<HealthComponent>();
			var amount = h.MaxHealth;

			if (amount == 0) {
				return;
			}
			
			entity.GetComponent<HeartsComponent>().ModifyShields(amount, item);
			h.InitMaxHealth = 0;
			
			TextParticle.Add(entity, Locale.Get("max_hp"), amount, true, true);
			TextParticle.Add(entity, Locale.Get("shields"), amount, true);
		}
	}
}