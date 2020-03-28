using System;
using BurningKnight.entity.component;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.desert {
	public class Spelunker : Mob {
		private const float DetectionRadius = 64f;
		private bool Exploded;
		
		protected override void SetStats() {
			base.SetStats();
			
			SetMaxHp(7);
			AddAnimation("spelunker");
			
			var body = new RectBodyComponent(3, 15, 10, 1);
			AddComponent(body);

			body.KnockbackModifier = 2;
			body.Body.LinearDamping = 6;

			AddComponent(new SensorBodyComponent(5, 4, 7, 11));
			
			Become<IdleState>();
		}
		
		#region Spelunker States
		public class IdleState : SmartState<Spelunker> {
			private float delay;

			public override void Init() {
				base.Init();
				delay = Rnd.Int(1, 2);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null && Self.DistanceTo(Self.Target) < DetectionRadius) {
					Become<RunState>();
					return;
				}

				if (T >= delay) {
					Become<WanderState>();
				}
			}
		}
		
		public class WanderState : SmartState<Spelunker> {
			private Vector2 velocity;
			private float timer;
			private float angle;
			
			public override void Init() {
				base.Init();

				angle = Rnd.AnglePI();
				timer = Rnd.Float(0.8f, 2f);
				
				var force = Rnd.Float(60f, 80f);
				
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

		public class RunState : SmartState<Spelunker> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					Self.Become<IdleState>();
					return;
				}

				var d = Self.DistanceTo(Self.Target);

				if (d <= 32f) {
					Self.Become<ExplodeState>();
				}
				
				if (d > 96f) {
					Self.Become<IdleState>();
					return;
				}

				var dx = Self.DxTo(Self.Target);
				var dy = Self.DyTo(Self.Target);

				var s = dt * 250;

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2(dx / d * s, dy / d * s);
				Self.PushFromOtherEnemies(dt);
			}
		}

		public class ExplodeState : SmartState<Spelunker> {
			private bool lastFlash;
			
			public override void Destroy() {
				base.Destroy();
				
				Self.GetComponent<MobAnimationComponent>().Flash = false;
			}

			public override void Update(float dt) {
				base.Update(dt);
								
				var d = Self.DistanceTo(Self.Target);

				if (d > 48f) {
					Self.Become<RunState>();
					return;
				}

				if (T >= 0.66f) {
					Self.Done = true;
					Self.Exploded = true;
					AudioEmitterComponent.Dummy(Self.Area, Self.Center).EmitRandomized("mob_archeolog_explosion", sz: 0.2f);
					ExplosionMaker.Make(Self, 48f);
				}

				var flash = T % 0.33 < 0.15f;

				if (flash && !lastFlash) {
					Self.GetComponent<AudioEmitterComponent>().Emit("mob_spelunker_beep", 1f, T);
				}
				
				lastFlash = flash;
				
				Self.GetComponent<MobAnimationComponent>().Flash = flash;
			}
		}
		#endregion

		protected override string GetDeadSfx() {
			return Exploded ? null : "mob_archeolog_death";
		}

		protected override string GetHurtSfx() {
			return "mob_archeolog_hurt";
		}
	}
}