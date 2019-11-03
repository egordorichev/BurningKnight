using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class Spelunker : Mob {
		private const float DetectionRadius = 64f;
		
		protected override void SetStats() {
			base.SetStats();
			
			SetMaxHp(7);
			AddAnimation("spelunker");
			
			var body = new RectBodyComponent(5, 4, 7, 11);
			AddComponent(body);

			body.KnockbackModifier = 2;
			body.Body.LinearDamping = 6;
			
			Become<IdleState>();
		}
		
		#region Spelunker States
		public class IdleState : SmartState<Spelunker> {
			private float delay;

			public override void Init() {
				base.Init();
				delay = Random.Int(1, 2);
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

				angle = Random.AnglePI();
				timer = Random.Float(0.8f, 2f);
				
				var force = Random.Float(60f, 80f);
				
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
					ExplosionMaker.Make(Self, 48f);
				}
				
				Self.GetComponent<MobAnimationComponent>().Flash = T % 0.33 < 0.15f;
			}
		}
		#endregion
	}
}