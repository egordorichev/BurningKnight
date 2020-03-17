using BurningKnight.entity.events;
using BurningKnight.entity.item.util;
using Lens.entity;

namespace BurningKnight.entity.buff {
	public class BuffedBuff : Buff {
		public const string Id = "bk:buffed";
		
		public BuffedBuff() : base(Id) {
			Duration = 10;
		}

		public override string GetIcon() {
			return "buffed";
		}

		public override void HandleEvent(Event e) {
			if (e is MeleeArc.CreatedEvent meae) {
				meae.Arc.Damage *= 2;
			} else if (e is ProjectileCreatedEvent pce) {
				pce.Projectile.Damage *= 2;
			} else if (e is HealthModifiedEvent hme) {
				if (hme.Amount < 0) {
					hme.Amount *= 0.5f;
				}
			}
			
			base.HandleEvent(e);
		}
	}
}