using System;
using BurningKnight.entity.component;
using Lens.entity.component.logic;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.desert {
	public class Mummy : Mob {
		private const float DetectionRadius = 70f;
		
		protected override void SetStats() {
			base.SetStats();
			
			SetMaxHp(4);
			AddAnimation("mummy");
			
			var body = new RectBodyComponent(2, 15, 10, 1);
			AddComponent(body);

			body.KnockbackModifier = 3;
			body.Body.LinearDamping = 6;
			
			AddComponent(new SensorBodyComponent(3, 3, 7, 13));
			
			Become<IdleState>();
		}
		
		#region Mummy States
		public class IdleState : SmartState<Mummy> {
			private float delay;

			public override void Init() {
				base.Init();
				delay = Rnd.Int(1, 2);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<StateComponent>().StateInstance is SummonedState) {
					return;
				}
				
				if (Self.Target != null && Self.DistanceTo(Self.Target) < DetectionRadius) {
					Become<RunState>();
					return;
				}

				if (T >= delay) {
					Become<WanderState>();
				}
			}
		}
		
		public class WanderState : SmartState<Mummy> {
			private Vector2 velocity;
			private float timer;
			private float angle;
			
			public override void Init() {
				base.Init();

				angle = Rnd.AnglePI();
				timer = Rnd.Float(0.8f, 2f);
				
				var force = Rnd.Float(40f, 60f);
				
				velocity.X = (float) Math.Cos(angle) * force;
				velocity.Y = (float) Math.Sin(angle) * force;

				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				Self.GetComponent<MobAnimationComponent>().Animation.Tag = "run";
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null && Self.DistanceTo(Self.Target) < DetectionRadius) {
					Become<RunState>();
					return;
				}
				
				if (timer <= T) {
					Become<IdleState>();
					return;
				}

				var v = velocity * Math.Min(1, timer - T * 0.4f);
				Self.GetComponent<RectBodyComponent>().Velocity = v;
			}
		}

		public class SummonedState : SmartState<Mummy> {
			public override void Init() {
				base.Init();
				
				Self.TouchDamage = 0;
				var a = Self.GetComponent<MobAnimationComponent>();
				a.Scale.X = 3f;
				a.Scale.Y = 0f;

				Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.5f, Ease.BackOut);
				Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.5f, Ease.BackOut).OnEnd = () => {
					Become<IdleState>();
				};
			}

			public override void Destroy() {
				base.Destroy();
				Self.TouchDamage = 1;
			}
		}
		
		public class RunState : SmartState<Mummy> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					Self.Become<IdleState>();

					return;
				}

				var d = Self.DistanceTo(Self.Target);

				if (d > 96f) {
					Self.Become<IdleState>();
					return;
				}

				var dx = Self.DxTo(Self.Target);
				var dy = Self.DyTo(Self.Target);

				var s = dt * 300;

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2(dx / d * s, dy / d * s);
				Self.PushFromOtherEnemies(dt);
			}
		}
		#endregion

		protected override string GetHurtSfx() {
			return "mob_mummy_hurt";
		}

		protected override string GetDeadSfx() {
			return "mob_mummy_death";
		}
	}
}