using System;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile.pattern {
	public static class ExpandingCirclePattern {
		public static ProjectilePatternController Make(int total, float radius, float speed, float delay, float expansionSpeed, float rotationDelay) {
			return (p, pt, i, dt) => {
				if (pt.T > delay) {
					radius += Math.Max(0, Math.Min(1, (pt.T - delay) / delay)) * expansionSpeed * dt;
				}

				var angle = Math.Max(0, Math.Min(1, (pt.T) / rotationDelay)) * pt.T * speed + ((float) i) / (total) * Math.PI * 2;
				p.Center = pt.Center + new Vector2((float) Math.Cos(angle) * radius, (float) Math.Sin(angle) * radius); 
			};
		}
	}
}