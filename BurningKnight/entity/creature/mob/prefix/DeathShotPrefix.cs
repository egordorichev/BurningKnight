using System;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.prefix {
	public class DeathShotPrefix : Prefix {
		private static Vector4 color = new Vector4(245 / 255f, 85 / 255f, 93 / 255f, 1);
		
		public override bool HandleEvent(Event e) {
			if (e is DiedEvent) {
				var am = 8;
			
				for (var i = 0; i < am; i++) {
					var a = Math.PI * 2 * (((float) i) / am);
					var projectile = Projectile.Make(Mob, "small", a, 3f);
					
					projectile.AddLight(32f, ProjectileColor.Red);
				}
			}
			
			return base.HandleEvent(e);
		}

		public override Vector4 GetColor() {
			return color;
		}
	}
}