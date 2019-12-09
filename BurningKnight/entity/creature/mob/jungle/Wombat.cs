using System;
using System.Collections.Generic;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.mob.jungle {
	public class Wombat : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("wombat");
			SetMaxHp(8);
			
			Become<IdleState>();

			Flying = true;

			var body = new RectBodyComponent(1, 9, 14, 1);
			AddComponent(body);
			
			body.Body.LinearDamping = 2;
			body.Body.Restitution = 1;
			body.Body.Friction = 0;

			AddComponent(new SensorBodyComponent(1, 2, 14, 8));
		}

		#region Flower States
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

				// fixme: velocity angle and not to player xd
				Self.GetComponent<RectBodyComponent>().Velocity = MathUtils.CreateVector(Self.Target.AngleTo(Self), 10f);
			}

			public override void Update(float dt) {
				base.Update(dt);
				sinceLast -= dt;
				
				if (sinceLast <= 0) {
					sinceLast = 0.2f;
					Projectile.Make(Self, "square", Self.AngleTo(Self.Target), 10, scale: 0.8f);

				}
				
				if (T < 5f) {
					var body = Self.GetComponent<RectBodyComponent>();
					body.Velocity += body.Velocity * (dt * 60);
				} else {
					Become<IdleState>();
				}
			}
		}
		#endregion
	}
}