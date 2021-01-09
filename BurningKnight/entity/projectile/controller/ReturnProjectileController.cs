using System;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.util;

namespace BurningKnight.entity.projectile.controller {
	public static class ReturnProjectileController {
		public static ProjectileCallbacks.UpdateCallback Make(Entity owner, float speed = 1f) {
			return (p, dt) => {
				var b = p.GetAnyComponent<BodyComponent>();
				var dx = p.DxTo(owner);
				var dy = p.DyTo(owner);
				var d = MathUtils.Distance(dx, dy);

				if (d <= 8f) {
					p.Break();
					return;
				}

				b.Velocity = MathUtils.CreateVector(MathUtils.LerpAngle(b.Velocity.ToAngle(), Math.Atan2(dy, dx), dt * 50 * speed), b.Velocity.Length());
			};
		}
	}
}