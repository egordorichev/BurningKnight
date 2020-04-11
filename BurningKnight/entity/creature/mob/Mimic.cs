using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob {
	public class Mimic : Slime {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("mimic"));
			SetMaxHp(30);

			var body = CreateBodyComponent();
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
			
			AddComponent(CreateSensorBodyComponent());
		}

		protected virtual BodyComponent CreateBodyComponent() {
			return new RectBodyComponent(2, 12, 10, 1);
		}

		protected virtual BodyComponent CreateSensorBodyComponent() {
			return new SensorBodyComponent(2, 16, 16, 1);
		}

		protected override void OnJump() {
			base.OnJump();

			for (var i = 0; i < 3; i++) {
				Timer.Add(() => {
					if (Target == null) {
						return;
					}
				
					GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");

					var a = AngleTo(Target) + Rnd.Float(-0.1f, 0.1f);
					var projectile = Projectile.Make(this, "circle", a, 9f);

					projectile.Spectral = true;
					projectile.Center = Center + MathUtils.CreateVector(a, 5f) - new Vector2(0, GetComponent<ZComponent>().Z);
					projectile.AddLight(32f, Projectile.RedLight);
				}, i * 0.3f);
			}
		}

		protected override void OnLand() {
			if (Target == null) {
				return;
			}
			
			var am = 16;
			GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");

			for (var i = 0; i < am; i++) {
				var a = Math.PI * 2 * (((float) i) / am);
				var projectile = Projectile.Make(this, "small", a, i % 2 == 0 ? 7f : 5f);
					
				projectile.Center = BottomCenter;
				projectile.AddLight(32f, Projectile.RedLight);
			}
		}
	}
}