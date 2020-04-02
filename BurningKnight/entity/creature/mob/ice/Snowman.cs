using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.ice {
	public class Snowman : Mob {
		private float t;
		
		protected override Color GetBloodColor() {
			return Snowball.BloodColor;
		}
		
		protected override void SetStats() {
			base.SetStats();

			Width = 24;
			Height = 22;

			t = Rnd.Float(4);

			SetMaxHp(30);
			
			var body = new RectBodyComponent(5, 21, 15, 1);
			AddComponent(body);
			body.Body.LinearDamping = 10;
			body.KnockbackModifier = 0.1f;
			
			AddComponent(new SensorBodyComponent(6, 2, 13, 19));
			AddComponent(new MobAnimationComponent("snowman") {
				ShadowOffset = 3
			});
			
			Become<IdleState>();

			TouchDamage = 0;
		}

		private void Fire() {
			if (Target == null) {
				return;
			}
			
			GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");
			var a = GetComponent<MobAnimationComponent>();
					
			Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
				Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

					Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
					Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f);
				
					var an = AngleTo(Target);
					var projectile = Projectile.Make(this, "carrot", an, 8f);
					
					projectile.Color = ProjectileColor.Orange;
					projectile.Center = Center + MathUtils.CreateVector(an, 4f);
					projectile.Controller += TargetProjectileController.Make(Target);
					projectile.HurtsEveryone = true;
					projectile.Damage = 15;
					
					projectile.AddLight(32f, projectile.Color);
				};
			};
		}

		#region Snowman States
		public class IdleState : SmartState<Snowman> {
			public override void Init() {
				base.Init();
				T = Self.t;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null || !Self.CanSeeTarget()) {
					T = Self.t;
					return;
				}
				
				if (T >= 5f) {
					T = 0;
					Self.Fire();
				}
			}
		}
		#endregion

		protected override void CreateGore(DiedEvent d) {
			var head = new Snowball();
			Area.Add(head);
			head.TopCenter = TopCenter;

			var body = new SnowmanBody();
			Area.Add(body);
			body.BottomCenter = BottomCenter;
		}
		
		protected override string GetHurtSfx() {
			return "mob_snowman_hurt";
		}

		protected override string GetDeadSfx() {
			return "mob_snowman_death";
		}
	}
}