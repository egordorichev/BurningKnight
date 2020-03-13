using BurningKnight.entity.component;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.ice {
	public class BigSnowball : Mob {
		protected override Color GetBloodColor() {
			return Snowball.BloodColor;
		}
		
		protected override void SetStats() {
			base.SetStats();

			Width = 20;
			Height = 20;

			SetMaxHp(10);
			
			var body = new RectBodyComponent(3, 19, 14, 1);
			AddComponent(body);
			body.Body.LinearDamping = 1;
			body.KnockbackModifier = 2f;
			body.Body.Restitution = 1;
			body.Body.Friction = 0;
			
			AddComponent(new SensorBodyComponent(2, 3, 16, 16));
			AddComponent(new MobAnimationComponent("big_snowball") {
				ShadowOffset = 3
			});
			
			Become<IdleState>();
		}

		#region Snowman States
		public class IdleState : SmartState<BigSnowball> {
			private float delay;

			public override void Init() {
				base.Init();
				delay = Rnd.Float(1f, 2f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<RollState>();
				}
			}
		}
		
		public class RollState : SmartState<BigSnowball> {
			private const float Accuracy = 0.2f;
			
			private Vector2 velocity;
			private float timer;
			private float lastBullet;
			private float angle;
			private bool fire;
			private float start;
			private bool saw;
			
			public override void Init() {
				base.Init();

				fire = Self.Target != null && Self.moveId % 2 == 0;
				angle = !fire ? Rnd.AnglePI() : Self.AngleTo(Self.Target);
				timer = fire ? 0.9f : Rnd.Float(0.8f, 2f);
				start = Rnd.Float(0f, 10f);
				
				var a = angle + Rnd.Float(-Accuracy, Accuracy);
				var force = fire ? 60 : 120;
				
				velocity.X = (float) Math.Cos(a) * force;
				velocity.Y = (float) Math.Sin(a) * force;

				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				Self.moveId++;
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (timer <= T) {
					Become<IdleState>();

					return;
				}

				var v = velocity * Math.Min(1, timer - T * 0.4f);
				Self.GetComponent<RectBodyComponent>().Velocity = v;
			}
		}
		#endregion
	}
}