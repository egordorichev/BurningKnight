using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class MakeProjectilesBoomerangUse : ItemUse {
		public override bool HandleEvent(Event e) {
			if (e is ProjectileCreatedEvent pce) {
				var projectile = pce.Projectile;
				ProjectileCallbacks.AttachUpdateCallback(projectile, BoomerangProjectileController.Make(pce.Owner));

				if (projectile.T > 0) {
					projectile.T *= 2;
				}
			}

			return base.HandleEvent(e);
		}
	}
}