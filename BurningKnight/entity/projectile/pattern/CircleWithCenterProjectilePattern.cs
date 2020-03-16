using System;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile.pattern {
	public static class CircleWithCenterProjectilePattern {
		public static ProjectilePatternController Make(float radius, float speed) {
			return (p, data, pt, i, dt) => {
				if (i == 0) {
					p.Center = pt.Center;
					return;
				}
				
				var angle = pt.T * speed + ((float) i) / (pt.Count - 1) * Math.PI * 2;
				p.Center = pt.Center + new Vector2((float) Math.Cos(angle) * radius, (float) Math.Sin(angle) * radius); 
			};
		}
	}
}