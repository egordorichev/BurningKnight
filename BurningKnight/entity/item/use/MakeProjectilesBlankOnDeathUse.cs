using BurningKnight.entity.events;
using BurningKnight.state;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesBlankOnDeathUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				pce.Projectile.OnDeath += (p, en, t) => {
					if (Rnd.Chance(30 + Run.Luck * 10)) {
						BlankMaker.Make(p.Center, p.Area, 32f);
					}
				};
			}
			
			return base.HandleEvent(e);
		}
	}
}