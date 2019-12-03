using BurningKnight.entity.events;
using BurningKnight.entity.projectile.controller;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesBoomerangUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				pce.Projectile.Controller += BoomerangProjectileController.Make(pce.Owner);
				pce.Projectile.Range = 256;
			}

			return base.HandleEvent(e);
		}
	}
}