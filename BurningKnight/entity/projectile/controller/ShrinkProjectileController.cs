namespace BurningKnight.entity.projectile.controller {
	public static class ShrinkProjectileController {
		public static ProjectileUpdateCallback Make(float speed = 1f) {
			var t = 0f;
			var dmg = -1f;
			
			return (p, dt) => {
				t += dt * speed;
				
				if (dmg < 0) {
					dmg = p.Damage;
				}

				if (t >= 0.05f) {
					var s = p.Scale - t * 4f / p.Scale; // (p.Scale > 1 ? 1f / p.Scale : p.Scale);
					t -= 0.05f;

					p.AdjustScale(s);
					p.Damage = dmg * p.Scale;

					if (p.Width <= 1 || p.Height <= 1f) {
						p.Break();
					}
				}
			};
		}
	}
}