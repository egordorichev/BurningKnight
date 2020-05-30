using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.pattern;
using Lens.util;
using Lens.util.math;
using Lens.util.timer;

namespace BurningKnight.entity.creature.mob.library {
	public class Book : Mob {
		protected override void SetStats() {
			base.SetStats();

			Width = 14;

			SetMaxHp(25);
			
			var body = new RectBodyComponent(0, 15, 14, 1);
			AddComponent(body);

			body.KnockbackModifier = 0.1f;
			body.Body.LinearDamping = 4f;
			
			AddComponent(new SensorBodyComponent(0, 0, 14, 16));
			AddComponent(new ZAnimationComponent("book"));
			AddComponent(new ZComponent {
				Float = true
			});
			
			Become<IdleState>();
		}

		#region Snowman States
		public class IdleState : SmartState<Book> {
			private float delay;

			public override void Init() {
				base.Init();
				delay = Rnd.Float(1f, 5f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null) {
					if (T >= delay && Self.CanSeeTarget()) {
						Become<ToRageState>();
					}
				} else {
					T = 0;
				}
			}
		}
		
		public class ToRageState : SmartState<Book> {
			public override void Init() {
				base.Init();
				Self.GetComponent<ZAnimationComponent>().Animation.Tag = "anim";
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 0.2f) {
					Become<RageState>();
				}
			}
		}
		
		public class ToIdleState : SmartState<Book> {
			public override void Init() {
				base.Init();
				Self.GetComponent<ZAnimationComponent>().Animation.Tag = "anim";
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 0.2f) {
					Become<IdleState>();
				}
			}
		}

		private const string Chars = "abcdefghijklmnopqrstuvwxyz";
		
		public class RageState : SmartState<Book> {
			private bool fired;
			private float delay;

			public override void Init() {
				base.Init();
				delay = Rnd.Float(1f, 2f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 0.2f && !fired) {
					fired = true;
					
					if (Self.Target != null) {
						var h = Self.Target.GetComponent<HealthComponent>();
						var c = h.Health <= 1 ? 'f' : Chars[Rnd.Int(Chars.Length)];
						var a = Self.AngleTo(Self.Target);
						var color = ProjectileColor.Rainbow[Rnd.Int(ProjectileColor.Rainbow.Length)];
						var sprite = Rnd.Chance(60) ? "circle" : "square";

						if (LetterTemplateData.Data.TryGetValue(c, out var data)) {
							var p = new ProjectilePattern(KeepShapePattern.Make(0)) {
								Position = Self.Center
							};

							Self.Area.Add(p);

							ProjectileTemplate.MakeFast(Self, sprite, Self.Center, a, (pr) => {
								p.Add(pr);
								pr.Color = color;
								pr.AddLight(32, color);
								
								pr.CanBeReflected = false;
								pr.BodyComponent.Angle = a;
							}, data, () => {
								Timer.Add(() => {
									p.Launch(a, Rnd.Float(30, 80));
									Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire_static");
								}, 0.2f);
							});
						}
					}
				}

				if (T >= delay || Self.Target == null) {
					Become<ToIdleState>();
				}
			}
		}
		#endregion
	}
}