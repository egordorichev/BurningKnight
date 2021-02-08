using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesBlankOnDeathUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				ProjectileCallbacks.AttachDeathCallback(pce.Projectile, (p, en, t) => {
					if (Rnd.Chance(20 + Run.Luck * 10)) {
						BlankMaker.Make(p.Center, p.Area, 18f);
					}
				});
			}
			
			return base.HandleEvent(e);
		}
	}
}