using System;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile.pattern {
	public static class CircleProjectilePattern {
		public static ProjectilePatternController Make(float radius, float speed) {
			return (p, data, pt, i, dt) => {
				var angle = pt.T * speed + ((float) i) / pt.Count * Math.PI * 2;
				p.Center = pt.Center + new Vector2((float) Math.Cos(angle) * radius, (float) Math.Sin(angle) * radius);
				
				if (p.BodyComponent?.Body != null) {
					p.BodyComponent.Body.Rotation = (float) angle;
				}
			};
		}
	}
}