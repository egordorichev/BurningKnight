using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using Lens.entity;
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
			AddDrops(new SingleDrop("bk:ice_skates", 0.01f));
			
			var body = new RectBodyComponent(3, 19, 10, 5);
			AddComponent(body);
			
			body.Body.LinearDamping = 1f;
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
				delay = Rnd.Float(1f, 5f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<RollState>();
				}
				
				var v = Self.GetComponent<RectBodyComponent>().Body.LinearVelocity;

				if (v.LengthSquared() > 15f) {
					Become<RollState>();
				}
			}
		}
		
		public class RollState : SmartState<BigSnowball> {
			private const float Accuracy = 0.2f;
			
			private float angle;
			
			public override void Init() {
				base.Init();

				if (Self.GetComponent<RectBodyComponent>().Body.LinearVelocity.LengthSquared() < 15f) {
					angle = Rnd.Chance() || Self.Target == null ? Rnd.AnglePI() : Self.AngleTo(Self.Target);

					var a = angle + Rnd.Float(-Accuracy, Accuracy);
					var force = 200;

					Vector2 velocity;

					velocity.X = (float) Math.Cos(a) * force;
					velocity.Y = (float) Math.Sin(a) * force;

					Self.GetComponent<RectBodyComponent>().Velocity = velocity;
					Self.GetComponent<MobAnimationComponent>().Animation.Reverse = velocity.Y < 0;
				}
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
				Self.GetComponent<MobAnimationComponent>().Animation.Reverse = false;
			}

			public override void Update(float dt) {
				base.Update(dt);

				var v = Self.GetComponent<RectBodyComponent>().Body.LinearVelocity;

				if (v.LengthSquared() < 15f) {
					Become<IdleState>();
					Self.GetComponent<RectBodyComponent>().Body.LinearVelocity = Vector2.Zero;
					
					return;
				}
				
				Self.GetComponent<MobAnimationComponent>().Animation.Reverse = v.Y < 0;
			}
		}
		#endregion

		public override bool ShouldCollide(Entity entity) {
			if (entity is BigSnowball) {
				return true;
			}
			
			return base.ShouldCollide(entity);
		}
	}
}