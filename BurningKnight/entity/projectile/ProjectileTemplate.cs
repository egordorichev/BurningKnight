using System;
using Lens.entity;
using Lens.util;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public static class ProjectileTemplate {
		public static void Make(Entity owner, string texture, Vector2 center, float angle, float speed, int bounce, int start, Action<Projectile> modifier,  params string[] data) {
			var height = data.Length;
			var width = 0;

			for (var i = 0; i < height; i++) {
				width = Math.Max(width, data[i].Length);
			}
			
			var c = new Vector2();
			var d = 0.2f;

			for (var x = 0; x < width; x++) {
				var x1 = x;

				Timer.Add(() => {
					for (var y = 0; y < height; y++) {
						if (data[y][x1] != ' ') {
							var p = Projectile.Make(owner, texture, angle, speed, true, bounce);
							p.Center = center + MathUtils.RotateAround(new Vector2(0, (y - height / 2f) * 10), angle, c);
							
							modifier?.Invoke(p);
						}
					}	
				}, (width - x + start) * d);
			}
		}

		public static void Write(string what, Entity owner, string texture, Vector2 center, float angle, float speed, int bounce) {
			what = what.ToLower();

			for (var i = 0; i < what.Length; i++) {
				var c = what[i];

				if (c == ' ') {
					continue;
				}

				if (!LetterTemplateData.Data.TryGetValue(c, out var data)) {
					continue;
				}
				
				Make(owner, texture, center, angle, speed, bounce, (what.Length - i) * 4, null, data);
			}
		}
	}
}