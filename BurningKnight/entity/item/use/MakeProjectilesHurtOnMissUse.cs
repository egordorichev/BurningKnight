using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item.util;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.state;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesHurtOnMissUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (Run.Depth == 0) {
				return base.HandleEvent(e);
			}
			
			if (e is ProjectileCreatedEvent pce) {
				if (!(pce.Projectile.Owner is Player)) {
					return false;
				}
				
				var hurt = false;
				
				pce.Projectile.OnCollision += (p, en) => {
					if (en.HasComponent<HealthComponent>() || (en is ProjectileLevelBody && Run.Level.Biome is IceBiome)) {
						hurt = true;
					}
					
					return false;
				};
				
				pce.Projectile.OnDeath += (p, en, t) => {
					if (!hurt) {
						Item.Owner.GetComponent<HealthComponent>().ModifyHealth(-1, Item);
					}
				};
			} else if (e is MeleeArc.CreatedEvent mac) {
				if (!(mac.Arc.Owner is Player)) {
					return false;
				}
				
				var hurt = false;

				mac.Arc.OnHurt += (m, en) => {
					if (en.HasComponent<HealthComponent>() || (en is ProjectileLevelBody && Run.Level.Biome is IceBiome)) {
						hurt = true;
					}
				};

				mac.Arc.OnDeath += (m) => {
					if (!hurt) {
						Item.Owner.GetComponent<HealthComponent>().ModifyHealth(-1, Item);
					}
				};
			}

			return base.HandleEvent(e);
		}
	}
}