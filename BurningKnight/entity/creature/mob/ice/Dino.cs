using System;
using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.ice {
	public class Dino : Mob {
		private const float FindTime = 3f;
		
		protected override void SetStats() {
			// todo: make him bounce of walls?
			base.SetStats();

			Width = 17;
			Height = 20;
			
			AddAnimation("dino");
			SetMaxHp(20);
			
			Become<IdleState>();
			
			var body = new RectBodyComponent(2, 19, 13, 1);
			AddComponent(body);

			body.Body.LinearDamping = 6;
			body.Body.Restitution = 1;
			body.Body.Friction = 0;
			
			AddComponent(new SensorBodyComponent(1, 3, 15, 17));
		}

		#region Dino States
		public class IdleState : SmartState<Dino> {
			private float delay;

			public override void Init() {
				base.Init();
				delay = Rnd.Float(0.4f, 1f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<RunState>();
				} else {
					if (Self.CanSeeTarget()) {
						Self.sawTime += dt;

						if (Self.sawTime >= FindTime) {
							Become<RunState>();
						}
					} else {
						Self.sawTime = 0;
					}
				}
			}
		}

		private bool fire;
		private float sawTime;

		public class RunState : SmartState<Dino> {
			private const float Accuracy = 0.2f;
			
			private Vector2 velocity;
			private float timer;
			private float lastBullet;
			private float angle;
			private float start;
			private bool saw;
			
			public override void Init() {
				base.Init();

				if (Self.fire = (Self.sawTime >= FindTime)) {
					Self.sawTime = 0;
				}

				angle = !Self.fire ? Rnd.AnglePI() : Self.AngleTo(Self.Target);
				timer = Self.fire ? 0.9f * 5 : Rnd.Float(0.8f, 2f);
				start = Rnd.Float(0f, 10f);
				
				var a = angle + Rnd.Float(-Accuracy, Accuracy);
				var force = Self.fire ? 30 : 60;
				
				velocity.X = (float) Math.Cos(a) * force;
				velocity.Y = (float) Math.Sin(a) * force;

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
					return;
				}

				var v = velocity * Math.Min(1, timer - T * 0.4f);
				Self.GetComponent<RectBodyComponent>().Velocity = v;

				if (!Self.fire) {
					if (Self.CanSeeTarget()) {
						Self.sawTime += dt;
					} else {
						Self.sawTime = 0;
					}

					return;
				}

				lastBullet -= dt;
				
				if (lastBullet <= 0) {
					lastBullet = 0.2f;

					if (!saw && !Self.CanSeeTarget()) {
						return;
					}

					saw = true;
					
					var an = angle + Rnd.Float(-Accuracy, Accuracy) + Math.Cos(T * 6f + start) * 0.1f;
					var a = Self.GetComponent<MobAnimationComponent>();
					
					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.2f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
						
						Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");
						
						var projectile = Projectile.Make(Self, "circle", an, 15, scale: Rnd.Float(0.5f, 1f));

						projectile.Color = Rnd.Chance(70) ? ProjectileColor.Orange : ProjectileColor.Red;
						projectile.AddLight(32f, projectile.Color);
						projectile.Center += MathUtils.CreateVector(angle, 8);

						AnimationUtil.Poof(projectile.Center);
					};
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
		
		protected override string GetHurtSfx() {
			return "mob_dino_hurt";
		}

		protected override string GetDeadSfx() {
			return "mob_dino_death";
		}
	}
}