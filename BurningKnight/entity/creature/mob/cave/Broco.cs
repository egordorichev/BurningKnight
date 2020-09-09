using System;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.cave {
	public class Broco : Mob {
			protected override void SetStats() {
			base.SetStats();

			Width = 12;
			Height = 11;
			
			AddAnimation("broco");
			SetMaxHp(Run.Depth);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 10, 8, 1);
			AddComponent(body);
			body.Body.LinearDamping = 10;

			AddComponent(new SensorBodyComponent(2, 2, 8, 9));

			moveId = Rnd.Int(0, 2);
			
			AddComponent(new LightComponent(this, 20, new Color(0.5f, 1f, 0.4f)));
			AddDrops(new SingleDrop("bk:emerald", 0.001f));
		}

		private int moveId;
		
		#region Bandit States
		public class IdleState : SmartState<Broco> {
			private float delay;
			private float fireDelay;
			private bool fired;
			private bool firedLaser;
			private Laser laser;

			public override void Init() {
				base.Init();

				delay = Rnd.Float(1.5f, 2.5f);
				fireDelay = (firedLaser = (Self.moveId % 2 == 0)) ? 3 : Rnd.Float(0.5f, delay - 0.5f);
				Self.moveId++;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (laser != null) {
					laser.Position = Self.Center;
				}

				if (firedLaser && T >= delay) {
					Become<RunState>();
					return;
				}

				if (!firedLaser && !fired && T >= fireDelay) {
					fired = true;

					if (!Self.CanSeeTarget()) {
						firedLaser = true;
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
								firedLaser = true;
								return;
							}
								
							Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire");
							
							var ac = 0.1f;
							var angle = Self.AngleTo(Self.Target);
							var projectile = Projectile.Make(Self, "circle", angle + Rnd.Float(-ac, ac), 9f);

							projectile.Color = ProjectileColor.White;
							projectile.Center += MathUtils.CreateVector(angle, 8f);
							projectile.AddLight(32f, projectile.Color);
							projectile.Spectral = true;

							AnimationUtil.Poof(projectile.Center);

							Timer.Add(() => {
								if (Self.Done) {
									return;
								}
								
								laser = Laser.Make(Self, 0, 0, scale: 2, range: 64);

								laser.LifeTime = 2f;
								laser.Color = ProjectileColor.Green;
								laser.Position = Self.Center;
								laser.Angle = angle + Rnd.Float(-ac * 3f, ac * 3f);

								Self.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_laser", 4);

								Timer.Add(() => {
									firedLaser = true;
									Become<RunState>();
								}, 2f);
							}, 0.8f);
						};
					};
				}
			}
		}
		
		public class RunState : SmartState<Broco> {
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