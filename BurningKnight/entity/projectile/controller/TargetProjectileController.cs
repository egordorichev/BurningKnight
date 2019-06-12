using System;
using BurningKnight.entity.component;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile.controller {
	public static class TargetProjectileController {
		public static ProjectileUpdateCallback Make(Entity target, float speed = 1f) {
			return (p, dt) => {
				if (target.Done) {
					p.Controller = null;
					return;
				}

				var b = p.GetAnyComponent<BodyComponent>();
				var d = b.Velocity.Length();
				var a = b.Velocity.ToAngle();

				a = (float) MathUtils.LerpAngle(a, p.AngleTo(target), dt * speed * 4);
				
				b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
			};
		}
	}
}