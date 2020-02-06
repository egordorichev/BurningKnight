using BurningKnight.assets.particle.custom;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item.util;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.entity.buff {
	public class RageBuff : Buff {
		public const string Id = "bk:rage";
		
		public RageBuff() : base(Id) {
			Duration = 10;
		}

		public override string GetIcon() {
			return "rage";
		}

		public override void Init() {
			base.Init();

			if (Entity is Player) {
				TextParticle.Add(Entity, Locale.Get("damage"), 1, true);
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (Entity is Player) {
				TextParticle.Add(Entity, Locale.Get("damage"), 1, true, true);
			}
		}

		public override void HandleEvent(Event e) {
			if (e is MeleeArc.CreatedEvent meae) {
				meae.Arc.Damage *= 2;
			} else if (e is ProjectileCreatedEvent pce) {
				pce.Projectile.Damage *= 2;
			}
			
			base.HandleEvent(e);
		}
	}
}