using System;
using System.Collections.Generic;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.bk;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.entity.projectile.pattern;
using BurningKnight.util;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss {
	public class IceQueen : Boss {
		private static Color tint = new Color(50, 60, 234, 200);
		
		public bool InSecondPhase {
			get {
				var p = GetComponent<HealthComponent>().Percent;
				return p > 0.33f && p <= 0.66f;
			}
		}

		public bool InThirdPhase => GetComponent<HealthComponent>().Percent <= 0.33f;
		public bool InFirstPhase => GetComponent<HealthComponent>().Percent > 0.66f;

		protected override void AddPhases() {
			base.AddPhases();
			
			HealthBar.AddPhase(0.33f);
			HealthBar.AddPhase(0.66f);
		}
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 10;
			Height = 19;
			
			AddComponent(new SensorBodyComponent(1, 2, 8, 17));

			var body = new RectBodyComponent(1, 18, 8, 1);
			AddComponent(body);

			body.KnockbackModifier = 0.5f;
			body.Body.LinearDamping = 3;

			AddAnimation("ice_queen");
			SetMaxHp(300);
		}
		
		public override void SelectAttack() {
			base.SelectAttack();
			Become<IdleState>();
		}

		protected override void OnTargetChange(Entity target) {
			base.OnTargetChange(target);

			if (target == null) {
				Become<FriendlyState>();
			}
		}

		private float lastFadingParticle;

		public override void Update(float dt) {
			base.Update(dt);

			if (Target != null) {
				GraphicsComponent.Flipped = Target.CenterX < CenterX;
			}
			
			lastFadingParticle -= dt;

			if (lastFadingParticle <= 0) {
				lastFadingParticle = 0.2f;

				var particle = new FadingParticle(GetComponent<MobAnimationComponent>().Animation.GetCurrentTexture(), tint);
				Area.Add(particle);

				particle.Depth = Depth - 1;
				particle.Center = Center;
			}
		}

		private int counter;
		
		#region Ice Queen States
		protected class IdleState : SmartState<IceQueen> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= (Self.InThirdPhase ? 1f : 3f)) {
					T = 0;

					switch (Self.counter) {
						case 0: {
							Become<ExplosiveBulletsState>();
							break;
						}

						case 1: {
							Become<SpamBulletsState>();
							break;
						}

						case 3: {
							if (Self.InThirdPhase) {
								Become<SpinCircleAttack>();
							} else {
								Become<MoveState>();
							}

							break;
						}

						case 2: case 4: {
							Become<MoveState>();
							break;
						}

						case 5: {
							Become<BulletCircleAttack>();
							break;
						}
						
						case 6: {
							Become<SpinCircleAttack>();
							break;
						}
					}

					// todo: more attacks for thirt phase
					Self.counter = (Self.counter + 1) % (Self.InSecondPhase ? 7 : (Self.InThirdPhase ? 7 : 5));
				}
			}
		}

		public class SpinCircleAttack : SmartState<IceQueen> {
			private float t;
			
			public override void Update(float dt) {
				base.Update(dt);

				t += dt;

				if (T >= 0.15f) {
					T = 0;
					
					var amount = 4;

					for (var i = 0; i < amount; i++) {
						var a = Math.PI * 2 * ((float) i / amount) + Math.Cos(t * 0.8f) * Math.PI;
						var projectile = Projectile.Make(Self, (t % 0.5f < 0.25f) ^ (i % 2 ==0) ? "small" : "big", a, 6f + t * 2);
						projectile.Color = t % 1f < 0.5f ? ProjectileColor.Blue : ProjectileColor.Purple;
					}
				}

				if (t >= 3f) {
					Become<IdleState>();
				}
			}
		}

		public class BulletCircleAttack : SmartState<IceQueen> {
			private List<Projectile> projectiles = new List<Projectile>();
			private bool second;

			public override void Destroy() {
				base.Destroy();

				foreach (var p in projectiles) {
					p.Break();
				}
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					Become<IdleState>();
					return;
				}
				
				if (T >= 0.15f) {
					T = 0;

					if (second) {
						var p = projectiles[0];
						projectiles.RemoveAt(0);

						Self.GetComponent<AudioEmitterComponent>().Emit("mob_fire_static", pitch: (projectiles.Count / 16f - 0.5f) * 2);

						if (!p.Done) {
							p.BodyComponent.Velocity = MathUtils.CreateVector(p.AngleTo(Self.Target), 200f);
						} else {
							p.Break();
						}

						if (projectiles.Count == 0) {
							Become<IdleState>();
						}
					} else {
						var p = Projectile.Make(Self, projectiles.Count % 2 == 0 ? "circle" : "small", Self.AngleTo(Self.Target), 0);

						p.PreventDespawn = true;
						p.Color = projectiles.Count % 2 == 0 ? ProjectileColor.Blue : ProjectileColor.Cyan;
						p.Center = Self.Center + MathUtils.CreateVector(projectiles.Count / 4f * Math.PI, 20 + projectiles.Count * 2);
						p.Depth = 1;
						Self.GetComponent<AudioEmitterComponent>().Emit("mob_flower_charging", pitch: projectiles.Count / 8f);
						projectiles.Add(p);
						
						if (projectiles.Count == 16) {
							second = true;
						}
					}
				}
			}
		}
		
		protected class ExplosiveBulletsState : SmartState<IceQueen> {
			private const int SmallCount = 4;
			private const int InnerCount = 8;
			
			private int count;

			public override void Init() {
				base.Init();
				T = 10f;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 1f) {
					T = 0;
					count++;

					var a = Self.AngleTo(Self.Target) + Rnd.Float(-0.1f, 0.1f) + (count == 1 ? 0 : Math.PI);
					var projectile = Projectile.Make(Self, "square", a, 15f);

					projectile.Color = ProjectileColor.Red;
					projectile.AddLight(32f, projectile.Color);
					projectile.Center += MathUtils.CreateVector(a, 8);
					projectile.CanBeBroken = false;
					projectile.CanBeReflected = false;

					var tt = 0f;

					projectile.Controller += (p, dtt) => {
						tt += dtt;

						if (tt >= 0.3f) {
							tt = 0;

							var s = projectile.BodyComponent.Velocity.Length();

							if (s < 3f) {
								return;
							}
							
							var z = Projectile.Make(Self, "small", a - Math.PI + Rnd.Float(-0.1f, 0.1f), s, scale: Rnd.Float(0.8f, 1.2f));
							z.Center = projectile.Center;
						}
					};

					projectile.Controller += SlowdownProjectileController.Make();
					projectile.OnDeath += (p, e, t) => {
						
						for (var i = 0; i < SmallCount; i++) {
							var an = (float) (((float) i) / SmallCount * Math.PI * 2);
						
							var pp = new ProjectilePattern(CircleProjectilePattern.Make(4.5f, 10 * (i % 2 == 0 ? 1 : -1))) {
								Position = p.Center
							};

							for (var j = 0; j < 4; j++) {
								var b = Projectile.Make(Self, "small");
								pp.Add(b);
								b.Color = ProjectileColor.Red;
								b.AddLight(32f, b.Color);
								b.CanBeReflected = false;
							}
				
							pp.Launch(an, 40);
							Self.Area.Add(pp);
						}

						for (var i = 0; i < InnerCount; i++) {
							var b = Projectile.Make(Self, "snowflake", Rnd.AnglePI(), Rnd.Float(10, 40), true, 1, null, Rnd.Float(0.5f, 1f));
						
							b.Color = ProjectileColor.Cyan;
							b.Center = p.Center;
							b.CanBeReflected = false;
							b.Controller += SlowdownProjectileController.Make(Rnd.Float(0.5f, 4f));
						}
					};
					
					if (count == 2) {
						Become<IdleState>();
					}
				}
			}
		}

		protected class SpamBulletsState : SmartState<IceQueen> {
			private const int InnerCount = 32;
			
			private int count;

			public override void Init() {
				base.Init();
				T = 10f;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 4f) {
					T = 0;
					count++;

					var a = Self.AngleTo(Self.Target) + Rnd.Float(-0.1f, 0.1f) + (count == 1 ? 0 : Math.PI);
					var projectile = Projectile.Make(Self, "big", a, 7f);

					projectile.Color = ProjectileColor.Blue;
					projectile.AddLight(32f, projectile.Color);
					projectile.Center += MathUtils.CreateVector(a, 8);
					projectile.CanBeBroken = false;
					projectile.CanBeReflected = false;

					projectile.OnDeath += (p, e, t) => {
						for (var i = 0; i < InnerCount; i++) {
							var b = Projectile.Make(Self, "snowflake", (float) i / InnerCount * Math.PI * 2, i % 2 == 0 ? 6 : 12);
						
							b.Color = ProjectileColor.Blue;
							b.Center = p.Center;
							b.CanBeReflected = false;
							b.Rotates = true;
							b.AddLight(32f, b.Color);
						}
					};
					
					if (count == 1) {
						Become<IdleState>();
					}
				}
			}
		}

		protected class MoveState : SmartState<IceQueen> {
			private Vector2 to; 
			
			public override void Init() {
				base.Init();
				to = Self.Target.Center;
			}

			public override void Destroy() {
				base.Destroy();

				var aa = Self.AngleTo(Self.Target);

				for (var j = 0; j < 2; j++) {
					var projectile = Projectile.Make(Self, "donut", aa + (j % 2 == 0 ? -1 : 1) * 0.3f, 14f, scale: 1.5f);

					projectile.Color = ProjectileColor.Green;
					projectile.AddLight(32f, projectile.Color);
					projectile.Center += MathUtils.CreateVector(aa, 8);
					projectile.CanBeBroken = false;
					projectile.CanBeReflected = false;

					projectile.OnDeath += (p, e, t) => {
						var v = p.GetAnyComponent<BodyComponent>().Velocity;
						var a = v.ToAngle() - (float) Math.PI;
						var s = v.Length();
						var c = p.HasComponent<CircleBodyComponent>();

						for (var i = 0; i < Rnd.Int(3, 5); i++) {
							Projectile.Make(Self, p.Slice, a + Rnd.Float(-1.4f, 1.4f), s * Rnd.Float(0.3f, 1.5f), c, -1, p,
								p.Scale * Rnd.Float(0.4f, 1.5f)).Center = p.Center;
						}
					};
				}
			}

			public override void Update(float dt) {
				base.Update(dt);

				var dx = Self.DxTo(to);
				var dy = Self.DyTo(to);
				var d = MathUtils.Distance(dx, dy);

				if (d <= 16f) {
					Become<IdleState>();
				} else {
					var s = 120 * dt / d;
					Self.Position += new Vector2(dx * s, dy * s);
				}
			}
		}
		#endregion

		/*public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player) {
					cse.Entity.GetComponent<BuffsComponent>().Add(new FrozenBuff {
						Duration = 3
					});
				}
			}
			
			return base.HandleEvent(e);
		}*/
	}
}