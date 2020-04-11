using System;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.controller;
using BurningKnight.entity.projectile.pattern;
using BurningKnight.util;
using Lens.entity;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.mob.boss {
	public class IceQueen : Boss {
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
		
		/*
		 * should move a lot
		 * 
		 * attacks:
		 *
		 * + 
		 */

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

		private int counter;
		
		#region Ice Queen States
		protected class IdleState : SmartState<IceQueen> {
			public override void Update(float dt) {
				base.Update(dt);

				// fixme: put back to 0.5;
				if (T >= 3.5f) {
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
					}

					Self.counter = (Self.counter + 1) % 2;
				}
			}
		}

		protected class ExplosiveBulletsState : SmartState<IceQueen> {
			private const int SmallCount = 4;
			private const int InnerCount = 16;
			
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

					var a = Self.AngleTo(Self.Target) + Rnd.Float(-0.1f, 0.1f);
					var projectile = Projectile.Make(Self, "circle", a, 7f);

					projectile.Color = ProjectileColor.Blue;
					projectile.AddLight(32f, projectile.Color);
					projectile.Center += MathUtils.CreateVector(a, 8);
					projectile.CanBeBroken = false;
					projectile.CanBeReflected = false;

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
								b.Color = ProjectileColor.Cyan;
								b.AddLight(32f, b.Color);
								b.CanBeReflected = false;
							}
				
							pp.Launch(an, 40);
							Self.Area.Add(pp);
						}

						for (var i = 0; i < InnerCount; i++) {
							var s = Rnd.Chance(40);
							var b = Projectile.Make(Self, "snowflake", Rnd.AnglePI(), Rnd.Float(2, 12), true, 1, null, Rnd.Float(0.5f, 1f));
						
							b.Color = ProjectileColor.Cyan;
							b.Center = p.Center;
							b.CanBeReflected = false;
							b.Controller += SlowdownProjectileController.Make();
							b.AddLight(s ? 16f : 32f, b.Color);
						}
					};
					
					if (count == 3) {
						Become<IdleState>();
					}
				}
			}
		}

		protected class SpamBulletsState : SmartState<IceQueen> {
			private const int InnerCount = 48;
			
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
					var projectile = Projectile.Make(Self, "circle", a, 7f);

					projectile.Color = ProjectileColor.Blue;
					projectile.AddLight(32f, projectile.Color);
					projectile.Center += MathUtils.CreateVector(a, 8);
					projectile.CanBeBroken = false;
					projectile.CanBeReflected = false;

					projectile.OnDeath += (p, e, t) => {
						for (var i = 0; i < InnerCount; i++) {
							var b = Projectile.Make(Self, "snowflake", (float) i / InnerCount * Math.PI * 2, i % 2 == 0 ? 6 : 12, bounce: 1);
						
							b.Color = ProjectileColor.Blue;
							b.Center = p.Center;
							b.CanBeReflected = false;
							b.AddLight(32f, b.Color);
						}
					};
					
					if (count == 2) {
						Become<IdleState>();
					}
				}
			}
		}
		#endregion

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity is Player) {
					cse.Entity.GetComponent<BuffsComponent>().Add(new FrozenBuff {
						Duration = 3
					});
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}