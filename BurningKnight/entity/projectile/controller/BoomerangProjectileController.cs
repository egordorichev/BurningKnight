using BurningKnight.entity.component;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile.controller {
	public static class BoomerangProjectileController {
		public static ProjectileUpdateCallback Make(Entity owner, float speed = 1f) {
			return (p, dt) => {
				var b = p.GetAnyComponent<BodyComponent>();
				var dx = p.DxTo(owner);
				var dy = p.DyTo(owner);
				var d = MathUtils.Distance(dx, dy);
				var s = dt * 250 * speed;

				b.Velocity += new Vector2(dx / d * s, dy / d * s);
				b.Angle = b.Velocity.ToAngle();
			};
		}
	}
}