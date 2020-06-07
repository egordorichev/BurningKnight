using System;

namespace BurningKnight.entity.projectile.controller {
	public static class ShrinkProjectileController {
		public static ProjectileUpdateCallback Make(float speed = 1f) {
			var t = 0f;
			var z = 0.05f;
			var dmg = -1f;
			var sc = 2f;
			
			return (p, dt) => {
				if (dmg < 0) {
					// sc = p.Scale;
					dmg = p.Damage;
				}
				
				t += dt * speed;
				z += dt;

				if (z >= 0.05f) {
					var s = sc - t * 4f / Math.Max(1, p.Scale); // (p.Scale > 1 ? 1f / p.Scale : p.Scale);
					z = 0;

					p.AdjustScale(s);
					p.Damage = dmg * p.Scale;

					if (p.Width <= 0 || p.Height <= 0) {
						p.Break();
					}
				}
			};
		}
	}
}