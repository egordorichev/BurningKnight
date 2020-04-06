using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesHurtOnMissUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				pce.Projectile.OnDeath += (p, en, t) => {
					if (p.Parent == null && (t || !(en is Creature))) {
						Item.Owner.GetComponent<HealthComponent>().ModifyHealth(-1, Item);
					}
				};
			}
			
			return base.HandleEvent(e);
		}
	}
}