using System;
using System.Collections.Generic;
using BurningKnight.assets.achievements;
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
			SetMaxHp(500);
		}

		private void Animate() {
			GetComponent<MobAnimationComponent>().Animate();
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

						case 7: {
							Become<MoveState>();
							break;
						}
					}

					Self.counter = (Self.counter + 1) % (Self.InSecondPhase ? 7 : (Self.InThirdPhase ? 8 : 5));
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

					var builder = new ProjectileBuilder(Self, "small");

					for (var i = 0; i < amount; i++) {
						var a = Math.PI * 2 * ((float) i / amount) + Math.Cos(t * 0.8f) * Math.PI;
						builder.Slice = (t % 0.5f < 0.25f) ^ (i % 2 == 0) ? "small" : "big";
						builder.Color = t % 1f < 0.5f ? ProjectileColor.Blue : ProjectileColor.Purple;

						builder.Shoot(a, 6f + t * 2).Build();
					}

					Self.Animate();
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
						var builder = new ProjectileBuilder(Self, projectiles.Count % 2 == 0 ? "circle" : "small");
						var p = builder.Shoot(Self.AngleTo(Self.Target), 0).Build();

						p.Color = projectiles.Count % 2 == 0 ? ProjectileColor.Blue : ProjectileColor.Cyan;
						p.Center = Self.Center + MathUtils.CreateVector(projectiles.Count / 4f * Math.PI, 20 + projectiles.Count * 2);
						p.Depth = 1;

						Self.GetComponent<AudioEmitterComponent>().Emit("mob_flower_charging", pitch: projectiles.Count / 8f);
						projectiles.Add(p);
						
						if (projectiles.Count == 16) {
							second = true;
						}
						
						Self.Animate();
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
					var builder = new ProjectileBuilder(Self, "square") {
						LightRadius = 32f,
						Range = 5
					};

					builder.RemoveFlags(ProjectileFlags.BreakableByMelee, ProjectileFlags.Reflectable);

					var projectile = builder.Shoot(a, 15f).Build();
					Self.Animate();

					projectile.Color = ProjectileColor.Red;
					projectile.Center += MathUtils.CreateVector(a, 8);

					var tt = 0f;

					ProjectileCallbacks.AttachUpdateCallback(projectile, (p, dtt) => {
						tt += dtt;

						if (tt >= 0.3f) {
							tt = 0;

							var s = projectile.BodyComponent.Velocity.Length();

							if (s < 3f) {
								return;
							}

							var bb = new ProjectileBuilder(Self, "small") {
								Scale = Rnd.Float(0.8f, 1.2f)
							};

							var z = bb.Shoot(a - Math.PI + Rnd.Float(-0.1f, 0.1f), s).Build();
							z.Center = projectile.Center;
						}
					});

					ProjectileCallbacks.AttachUpdateCallback(projectile, SlowdownProjectileController.Make());

					ProjectileCallbacks.AttachDeathCallback(projectile, (p, e, t) => {
						for (var i = 0; i < SmallCount; i++) {
							var an = (float) (((float) i) / SmallCount * Math.PI * 2);
						
							var pp = new ProjectilePattern(CircleProjectilePattern.Make(4.5f, 10 * (i % 2 == 0 ? 1 : -1))) {
								Position = p.Center
							};

							var bb = new ProjectileBuilder(Self, "small") {
								LightRadius = 32f
							};

							bb.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

							for (var j = 0; j < 4; j++) {
								var b = bb.Build();
								pp.Add(b);
								b.Color = ProjectileColor.Red;
							}
				
							pp.Launch(an, 40);
							Self.Area.Add(pp);
						}

						var bbb = new ProjectileBuilder(Self, "snowflake") {
							Color = ProjectileColor.Cyan
						};

						bbb.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

						for (var i = 0; i < InnerCount; i++) {
							bbb.Scale = Rnd.Float(0.5f, 1f);
							var b = bbb.Shoot(Rnd.AnglePI(), Rnd.Float(10, 40)).Build();
						
							b.Center = p.Center;
							ProjectileCallbacks.AttachUpdateCallback(b,  SlowdownProjectileController.Make(Rnd.Float(0.5f, 4f)));
						}
					});
					
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
					var builder = new ProjectileBuilder(Self, "big") {
						LightRadius = 32f
					};

					builder.RemoveFlags(ProjectileFlags.BreakableByMelee, ProjectileFlags.Reflectable);

					var projectile = builder.Shoot(a, 7).Build();
					Self.Animate();

					projectile.Color = ProjectileColor.Blue;
					projectile.Center += MathUtils.CreateVector(a, 8);

					ProjectileCallbacks.AttachDeathCallback(projectile, (p, e, t) => {
						var bb = new ProjectileBuilder(Self, "snowflake") {
							Color = ProjectileColor.Blue,
							LightRadius = 32
						};

						bb.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);
						bb.AddFlags(ProjectileFlags.AutomaticRotation);

						for (var i = 0; i < InnerCount; i++) {
							var b = bb.Shoot((float) i / InnerCount * Math.PI * 2, i % 2 == 0 ? 6 : 12).Build();
							b.Center = p.Center;
						}
					});
					
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
				Self.Animate();
			}

			public override void Destroy() {
				base.Destroy();

				var aa = Self.AngleTo(Self.Target);
				Self.Animate();

				var builder = new ProjectileBuilder(Self, "donut") {
					Scale = 1.5f,
					Color = ProjectileColor.Green,
					LightRadius = 32f
				};

				builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

				for (var j = 0; j < 2; j++) {
					var projectile = builder.Shoot(aa + (j % 2 == 0 ? -1 : 1) * 0.3f, 14f).Build();

					projectile.Center += MathUtils.CreateVector(aa, 8);

					ProjectileCallbacks.AttachDeathCallback(projectile, (p, e, t) => {
						var v = p.GetAnyComponent<BodyComponent>().Velocity;
						var a = v.ToAngle() - (float) Math.PI;
						var s = v.Length();
						var c = p.HasComponent<CircleBodyComponent>();

						var b = new ProjectileBuilder(Self, p.Slice) {
							Parent = p,
							Scale = p.Scale * Rnd.Float(0.4f, 1.5f)
						};

						for (var i = 0; i < Rnd.Int(3, 5); i++) {
							b.Shoot(a + Rnd.Float(-1.4f, 1.4f), s * Rnd.Float(0.3f, 1.5f)).Build().Center = p.Center;
						}
					});
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

		public override void PlaceRewards() {
			base.PlaceRewards();
			Achievements.Unlock("bk:ice_boss");
		}

		public override string GetScream() {
			return "ice_queen_scream";
		}
	}
}