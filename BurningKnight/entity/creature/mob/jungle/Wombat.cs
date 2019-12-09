using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.mob.jungle {
	public class Wombat : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("wombat");
			SetMaxHp(6);
			
			Become<IdleState>();

			Flying = true;
			Height = 12;

			GetComponent<MobAnimationComponent>().ShadowOffset = -2;

			var body = new RectBodyComponent(1, 9, 14, 1);
			AddComponent(body);
			
			body.Body.LinearDamping = 2;
			body.Body.Restitution = 1;
			body.Body.Friction = 0;

			AddComponent(new SensorBodyComponent(1, 2, 14, 8));
		}

		#region Wombat States
		public class IdleState : SmartState<Wombat> {
			public override void Init() {
				base.Init();
				T = Rnd.Float(1);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					T = 0;
				} else if (T >= 5f) {
					Become<FireState>();
				}
			}
		}

		public class FireState : SmartState<Wombat> {
			private float sinceLast;
			
			public override void Init() {
				base.Init();

				if (Self.Target == null) {
					Become<FireState>();
					return;
				}

				Self.GetComponent<RectBodyComponent>().Velocity = MathUtils.CreateVector(Self.Target.AngleTo(Self), 10f);
			}

			public override void Update(float dt) {
				base.Update(dt);
				sinceLast -= dt;
				
				if (sinceLast <= 0) {
					sinceLast = 0.2f;
					Projectile.Make(Self, "square", Self.GetComponent<RectBodyComponent>().Velocity.ToAngle() - Math.PI + Rnd.Float(-0.2f, 0.2f), Rnd.Float(4, 7), scale: Rnd.Float(0.4f, 0.8f));
				}
				
				if (T < 5f) {
					var body = Self.GetComponent<RectBodyComponent>();
					body.Velocity += body.Velocity * (dt * 40);
				} else {
					Become<IdleState>();
				}
			}
		}
		#endregion
	}
}