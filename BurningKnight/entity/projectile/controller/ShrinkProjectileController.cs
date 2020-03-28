namespace BurningKnight.entity.projectile.controller {
	public static class ShrinkProjectileController {
		public static ProjectileUpdateCallback Make(float speed = 1f) {
			var t = 0f;
			
			return (p, dt) => {
				t += dt * speed;

				if (t >= 0.2f) {
					var s = p.Scale - t * 0.5f;

					if (s <= 0.1f) {
						p.Break();
						return;
					}
					
					p.AdjustScale(s);
				}
			};
		}
	}
}