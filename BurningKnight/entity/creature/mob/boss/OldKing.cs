using System;
using BurningKnight.assets.achievements;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.entity.projectile.pattern;
using BurningKnight.level;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss {
	public class OldKing : Boss {
		public bool Raging => GetComponent<HealthComponent>().Percent <= 0.25f;

		protected override void AddPhases() {
			base.AddPhases();
			HealthBar.AddPhase(0.25f);
		}

		public override void AddComponents() {
			base.AddComponents();

			Width = 19;
			Height = 27;
			
			AddComponent(new SensorBodyComponent(2, 10, 14, 17));

			var body = new RectBodyComponent(2, 26, 14, 1);
			AddComponent(body);

			body.KnockbackModifier = 0.05f;
			body.Body.LinearDamping = 1;

			AddComponent(new ZComponent {
				Gravity = 2
			});
			
			AddComponent(new ZAnimationComponent("old_king"));
			SetMaxHp(100);
		}

		private float lastParticle;

		public override void Update(float dt) {
			base.Update(dt);

			lastParticle -= dt;

			if (lastParticle <= 0) {
				lastParticle = 0.1f;

				if (!IsFriendly()) {
					Area.Add(new FireParticle {
						Offset = new Vector2(-2, -13),
						Owner = this,
						Size = 0.5f,
						Depth = Depth + 1
					});

					Area.Add(new FireParticle {
						Offset = new Vector2(2, -13),
						Owner = this,
						Size = 0.5f,
						Depth = Depth + 1
					});
				}
			}
		}

		public override bool ShouldCollide(Entity entity) {
			if (entity is Chasm) {
				return true;
			}
			
			return base.ShouldCollide(entity);
		}

		public override bool InAir() {
			var state = GetComponent<StateComponent>().StateInstance;
			
			return state is UpState || state is DownState;
		}
		
		#region Old King States
		private int lastAttack;

		protected override void OnTargetChange(Entity target) {
			base.OnTargetChange(target);

			if (target == null) {
				Become<FriendlyState>();
			}
		}

		public class IdleState : SmartState<OldKing> {
			public override void Update(float dt) {
				if (Self.Target == null) {
					return;
				}

				base.Update(dt);

				if (T >= (Self.Raging ? 1f : 2f)) {
					if (Rnd.Chance(95)) {
						Self.lastAttack = (Self.lastAttack + 1) % 2;
					}

					if (Self.lastAttack == 0) {
						Become<SkullAttack>();
					} else {
						Become<JumpState>();
					}
				}
			}
		}
		
		public class SkullAttack : SmartState<OldKing> {
			private int count;

			public override void Update(float dt) {
				base.Update(dt);

				if ((count + 1) * (Self.Raging ? 0.7f : 1.5f) <= T) {
					count++;

					if (Self.Target == null || Self.Died) {
						return;
					}
					
					var a = Self.GetComponent<ZAnimationComponent>();
					Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_oldking_shoot");

					Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {

						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.3f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.3f);

						if (Self.Target == null || Self.Died) {
							return;
						}

						var builder = new ProjectileBuilder(Self, "skull") {
							Range = 5f
						};

						builder.RemoveFlags(ProjectileFlags.Reflectable);
						var skull = builder.Shoot(Self.AngleTo(Self.Target), 8).Build();

						ProjectileCallbacks.AttachDeathCallback(skull, (p, e, t) => {
							if (!t) {
								return;
							}

							var b = new ProjectileBuilder(Self, "small");

							b.RemoveFlags(ProjectileFlags.Reflectable);
					
							for (var i = 0; i < 8; i++) {
								var bullet = b.Shoot(((float) i) / 4 * (float) Math.PI, (i % 2 == 0 ? 2 : 1) * 4 + 3).Build();
								bullet.Center = p.Center;
							}
						});

						ProjectileCallbacks.AttachUpdateCallback(skull, TargetProjectileController.Make(Self.Target, 0.5f));
						skull.GetComponent<ProjectileGraphicsComponent>().IgnoreRotation = true;
						
						if (count == (Self.Raging ? 6 : 4)) {
							Self.Become<IdleState>();
						}
					};
				}
			}
		}
		
		public class JumpState : SmartState<OldKing> {
			public override void Init() {
				base.Init();
				Self.GetComponent<ZAnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<ZAnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<ZAnimationComponent>().Animation.Paused) {
					Become<UpState>();
					Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_oldking_jump");
				}
			}
		}
		
		public class UpState : SmartState<OldKing> {
			public override void Init() {
				base.Init();
				
				var a = Self.Target == null || (!Self.Raging && Rnd.Chance()) ? Rnd.AnglePI() : Self.AngleTo(Self.Target) + Rnd.Float(-0.1f, 0.1f);
				var force = Rnd.Float(20f) + (Self.Raging ? 240 : 120);
				
				Self.GetComponent<RectBodyComponent>().Velocity = new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
				Self.GetComponent<ZComponent>().ZVelocity = 10;
				
				Self.TouchDamage = 0;
				Self.Depth = Layers.FlyingMob;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<ZComponent>().ZVelocity <= 0) {
					Become<DownState>();
				}
			}
		}
		
		public class DownState : SmartState<OldKing> {
			public override void Destroy() {
				base.Destroy();				
				
				Self.TouchDamage = 1;				
				Self.Depth = Layers.Creature;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<ZComponent>().Z <= 0) {
					Become<LandState>();
				}
			}
		}

		private int jumpCounter;
		
		public class LandState : SmartState<OldKing> {
			private const int SmallCount = 8;
			private const int InnerCount = 8;

			public override void Init() {
				base.Init();
				
				var a = Self.GetComponent<ZAnimationComponent>();
				a.SetAutoStop(true);

				
				Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_oldking_land");
				
				Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
					Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.3f);
					Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.3f);
				};
				
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
				var bb = new ProjectileBuilder(Self, "small");

				bb.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

				for (var i = 0; i < SmallCount; i++) {
					var an = (float) (((float) i) / SmallCount * Math.PI * 2);
						
					var pp = new ProjectilePattern(CircleProjectilePattern.Make(4.5f, 10 * (i % 2 == 0 ? 1 : -1))) {
						Position = Self.BottomCenter
					};

					for (var j = 0; j < 2; j++) {
						pp.Add(bb.Build());
					}
				
					pp.Launch(an, 40);
					Self.Area.Add(pp);
				}

				var aa = Self.AngleTo(Self.Target);
				var builder = new ProjectileBuilder(Self, "small") {
					Color = ProjectileColor.Green
				};

				builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);
					
				for (var i = 0; i < InnerCount; i++) {
					builder.Scale = Rnd.Float(0.5f, 1f);
					var b = builder.Shoot(aa + Rnd.Float(-0.3f, 0.3f), Rnd.Float(2, 12)).Build();
						
					b.Center = Self.BottomCenter;
				}
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<ZAnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				var animation = Self.GetComponent<ZAnimationComponent>().Animation;

				if (animation.Paused) {
					if (Self.Raging) {
						if (Self.jumpCounter < 2) {
							Become<JumpState>();
							Self.jumpCounter++;

							return;
						}

						Self.jumpCounter = 0;
					}

					Become<IdleState>();
				}
			}
		}
		#endregion

		public override void SelectAttack() {
			base.SelectAttack();
			Become<IdleState>();
		}

		public override void PlaceRewards() {
			base.PlaceRewards();
			Achievements.Unlock("bk:democracy");
		}
	}
}