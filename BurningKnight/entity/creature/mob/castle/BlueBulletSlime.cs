using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.graphics;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class BlueBulletSlime : BulletSlime {
		private static readonly Color color = ColorUtils.FromHex("#00cdf9");
		
		protected override Color GetBloodColor() {
			return color;
		}

		protected override void SetStats() {
			base.SetStats();
			SetMaxHp(4);
		}

		protected override string GetSprite() {
			return "blue_bullet_slime";
		}

		protected override void DoSpit() {
			var am = 8;
			GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");

			for (var i = 0; i < am; i++) {
				var a = Math.PI * 2 * (((float) i) / am);
				var projectile = Projectile.Make(this, "small", a + Rnd.Float(-0.1f, 0.1f), 5f);
					
				projectile.Center = BottomCenter;
				projectile.Color = ProjectileColor.Cyan;
				projectile.CanBeReflected = false;
				projectile.AddLight(32f, ProjectileColor.Cyan);
			}

			Timer.Add(() => {
				if (GetComponent<HealthComponent>().Dead) {
					return;
				}

				GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");
				GetComponent<ZAnimationComponent>().Animate();

				for (var i = 0; i < am; i++) {
					var a = Math.PI * 2 * (((float) i) / am + 0.5f);
					var projectile = Projectile.Make(this, "circle", a + Rnd.Float(-0.1f, 0.1f), 8f);
					
					projectile.Center = BottomCenter;
					projectile.Color = ProjectileColor.Blue;
					projectile.Spectral = true;
					projectile.CanBeReflected = false;
					projectile.AddLight(48f, ProjectileColor.Blue);
				}
			}, 1f);
		}
	}
}