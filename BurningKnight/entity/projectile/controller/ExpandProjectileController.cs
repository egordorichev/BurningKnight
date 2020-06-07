using System;

namespace BurningKnight.entity.projectile.controller {
	public class ExpandProjectileController {
		public static ProjectileUpdateCallback Make(float speed = 1f) {
			var t = 0f;
			var z = 0f;
			var dmg = -1f;
			
			return (p, dt) => {
				t += dt * speed;
				z += dt;
				
				if (dmg < 0) {
					dmg = p.Damage;
				}

				if (z >= 0.05f) {
					var s = Math.Min(1, t * 5f); // (p.Scale > 1 ? 1f / p.Scale : p.Scale);
					z -= 0.05f;

					p.AdjustScale(s);
					p.Damage = dmg * p.Scale;
				}
			};
		}
	}
}