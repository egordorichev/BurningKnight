using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class BulletSlime : Slime {
		private static readonly Color color = ColorUtils.FromHex("#ff0000");
		
		protected override Color GetBloodColor() {
			return color;
		}

		protected override float GetJumpDelay() {
			return 1 + Rnd.Float(0.5f, 1.2f);
		}

		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent(GetSprite()));
			SetMaxHp(1);

			var body = new RectBodyComponent(1, 15, 14, 1);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
			
			AddComponent(new SensorBodyComponent(2, 7, 12, 9));
		}

		protected virtual string GetSprite() {
			return "bullet_slime";
		}

		protected override void OnLand() {
			base.OnLand();

			if (Target == null) {
				return;
			}
			
			var am = 8;
			GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");

			for (var i = 0; i < am; i++) {
				var a = Math.PI * 2 * (((float) i) / am);
				var projectile = Projectile.Make(this, "small", a, 5f);
					
				projectile.Center = BottomCenter;
				projectile.AddLight(32f, Projectile.RedLight);
			}
		}
	}
}