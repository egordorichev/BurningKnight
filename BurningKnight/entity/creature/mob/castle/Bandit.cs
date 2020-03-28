using System;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class Bandit : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("bandit");
			SetMaxHp(Run.Depth);
			
			Become<IdleState>();

			var body = new RectBodyComponent(3, 13, 10, 1);
			AddComponent(body);
			body.Body.LinearDamping = 10;

			AddComponent(new SensorBodyComponent(2, 2, 12, 12));

			moveId = Rnd.Int(0, 2);
		}

		private int moveId;
		
		#region Bandit States
		public class IdleState : SmartState<Bandit> {
			private float delay;
			private float fireDelay;
			private bool fired;

			public override void Init() {
				base.Init();

				delay = Rnd.Float(1, 2.5f);
				fireDelay = Self.moveId % 2 == 0 ? 3 : Rnd.Float(0.5f, delay - 0.5f);
				Self.moveId++;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<RunState>();
					return;
				}

				if (!fired && T >= fireDelay) {
					fired = true;

					if (!Self.CanSeeTarget()) {
						return;
					}

					var a = Self.GetComponent<MobAnimationComponent>();
					
					Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {

						Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
						Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

							Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
							Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f);

							if (Self.Target == null) {
								return;
							}
								
							Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");
							
							var ac = 0.1f;
							var angle = Self.AngleTo(Self.Target) + Rnd.Float(-ac, ac);
							var projectile = Projectile.Make(Self, "small", angle, 8f);

							projectile.Center += MathUtils.CreateVector(angle, 8f);
							projectile.AddLight(32f, Projectile.RedLight);
							projectile.Spectral = true;

							AnimationUtil.Poof(projectile.Center);
						};
					};
				}
			}
		}
		
		public class RunState : SmartState<Bandit> {
			private Vector2 velocity;
			private float timer;
			
			public override void Init() {
				base.Init();

				timer = Rnd.Float(0.4f, 1f);
				
				var angle = Rnd.AnglePI();
				var force = 120f + Rnd.Float(50f);
				
				if (Rnd.Chance() && Self.Target != null) {
					var ac = 0.1f;
					angle = Self.AngleTo(Self.Target) + Rnd.Float(-ac, ac);
				}
				
				velocity.X = (float) Math.Cos(angle) * force;
				velocity.Y = (float) Math.Sin(angle) * force;

				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);
				
				if (timer <= T) {
					Become<IdleState>();
				} else {
					Self.GetComponent<RectBodyComponent>().Velocity = velocity * Math.Min(1, timer - T * 0.4f);
				}
			}
		}
		#endregion

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev) {
				if (ev.Entity is Door) {
					var s = GetComponent<StateComponent>().StateInstance;

					if (s is RunState) {
						Become<IdleState>();
					}
				}
			}
			
			return base.HandleEvent(e);
		}

		protected override string GetDeadSfx() {
			return "mob_bandit_death";
		}

		protected override string GetHurtSfx() {
			return "mob_bandit_damage";
		}
	}
}