using Lens.graphics;

namespace BurningKnight.entity.projectile.controller {
	public class HsvProjectileController {
		public static ProjectileCallbacks.UpdateCallback Make(float speed = 1f, float start = 0) {
			return (p, dt) => {
				p.Color = ColorUtils.FromHSV((p.T + start) * speed * 120 % 360, 100, 100);
			};
		}
	}
}