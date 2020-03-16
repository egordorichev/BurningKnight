using Lens.util;

namespace BurningKnight.entity.projectile.pattern {
	public static class KeepShapePattern {
		public static ProjectilePatternController Make(float speed) {
			return (p, data, pt, i, dt) => {
				var angle = pt.T * speed;
				p.Center = pt.Center + MathUtils.CreateVector(angle + data.Angle, data.Distance);
			};
		}
	}
}