using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.ice {
	public class SnowmanBody : Mob {
		private float t;

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		protected override Color GetBloodColor() {
			return Snowball.BloodColor;
		}
		
		protected override void SetStats() {
			base.SetStats();

			Width = 24;
			Height = 15;

			SetMaxHp(20);
			
			var body = new RectBodyComponent(4, 14, 17, 1);
			AddComponent(body);
			body.Body.LinearDamping = 10;
			body.KnockbackModifier = 0.1f;
			
			AddComponent(new SensorBodyComponent(5, 2, 15, 12));
			AddComponent(new MobAnimationComponent("snowman_body") {
				ShadowOffset = 3
			});
			
			GetComponent<AudioEmitterComponent>().PitchMod = -0.2f;
			
			Become<IdleState>();

			TouchDamage = 0;
		}

		private void Fire() {
			if (Target == null) {
				return;
			}
			
			GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");
			var a = GetComponent<MobAnimationComponent>();
					
			Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
			Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

				Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
				Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f);
			
				var an = AngleTo(Target) + t * 0.3f;
				var d = GetComponent<HealthComponent>().Percent <= 0.3f;

				var builder = new ProjectileBuilder(this, "carrot");

				for (var i = 0; i < (d ? 8 : 4); i++) {
					var projectile = builder.Shoot(an + i * Math.PI * (d ? 0.25f : 0.5f), 4f).Build();

					projectile.Color = d ? ProjectileColor.Red : ProjectileColor.Orange;
					projectile.Center = Center + MathUtils.CreateVector(an, 4f);
				}
			};
		}

		#region Snowman body States
		public class IdleState : SmartState<SnowmanBody> {
			public override void Update(float dt) {
				base.Update(dt);
				
				if (T >= 0.6f) {
					T = 0;
					Self.Fire();
				}
			}
		}
		#endregion
		
		protected override string GetHurtSfx() {
			return "mob_snowman_hurt";
		}

		protected override string GetDeadSfx() {
			return "mob_snowman_death";
		}
	}
}