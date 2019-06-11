using System;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public static class ProjectileTemplate {
		public static void Make(Entity owner, string texture, Vector2 center, float angle, float speed, int bounce, params string[] data) {
			var height = data.Length;
			var width = 0;

			for (var i = 0; i < height; i++) {
				width = Math.Max(width, data[i].Length);
			}
			
			var c = new Vector2();

			for (var y = 0; y < height; y++) {
				var d = data[y];
				
				for (var x = 0; x < d.Length; x++) {
					if (d[x] != ' ') {
						var p = Projectile.Make(owner, texture, angle, speed, true, bounce);

						p.Center = center + MathUtils.RotateAround(new Vector2((x - width / 2f) * 16, (y - height / 2f) * 16), angle, c);
					}
				}
			}
		}
	}
}