using System;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class DeathShotPrefix : Prefix {
		private static Vector4 color = new Vector4(245 / 255f, 85 / 255f, 93 / 255f, 1);
		
		public override bool HandleEvent(Event e) {
			if (e is DiedEvent) {
				var am = 8;

				var builder = new ProjectileBuilder(Mob, "small") {
					LightRadius = 32
				};
			
				for (var i = 0; i < am; i++) {
					var a = Math.PI * 2 * (((float) i) / am) + Rnd.Float(-0.1f, 0.1f);
					builder.Shoot(a, 3f).Build();
				}
			}
			
			return base.HandleEvent(e);
		}

		public override Vector4 GetColor() {
			return color;
		}
	}
}