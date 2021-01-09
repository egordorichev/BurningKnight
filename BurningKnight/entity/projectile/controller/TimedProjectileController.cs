using System;
using BurningKnight.assets.particle.custom;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile.controller {
	public static class TimedProjectileController {
		public static ProjectileCallbacks.UpdateCallback Make(float time, Action<Projectile> action) {
			var t = 0f;
			
			return (p, dt) => {
				t += dt;

				if (t >= time) {
					t = 0;

					action(p);
				}
			};
		}

		public static ProjectileCallbacks.UpdateCallback MakeFadingParticles(float time, Color tint) {
			return Make(time, p => {
				var pr = new FadingParticle(p.GetComponent<ProjectileGraphicsComponent>().Sprite, tint);
				p.Area.Add(pr);
				pr.Center = p.Center;
			});
		}
	}
}