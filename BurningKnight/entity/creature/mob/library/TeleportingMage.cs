using System;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.entity.projectile.pattern;
using Lens.util.math;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Utilities;
using MathUtils = Lens.util.MathUtils;

namespace BurningKnight.entity.creature.mob.library {
	public class TeleportingMage : Mob {
		private Color color;
		
		protected override void SetStats() {
			base.SetStats();

			Width = 10;
			Height = 14;

			SetMaxHp(20);
			
			var body = new RectBodyComponent(0, 13, 10, 1);
			AddComponent(body);

			body.KnockbackModifier = 0f;
			body.Body.LinearDamping = 4f;
			
			AddComponent(new SensorBodyComponent(1, 1, 8, 13));
			AddComponent(new MobAnimationComponent("mage"));
			
			Become<IdleState>();

			color = ProjectileColor.Rainbow[Rnd.Int(ProjectileColor.Rainbow.Length)];
		}

		#region Snowman States
		public class IdleState : SmartState<TeleportingMage> {
			private float delay;
			private bool tweened;

			public override void Init() {
				base.Init();
				delay = Rnd.Float(4, 10f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null) {
					Self.GraphicsComponent.Flipped = Self.Target.CenterX < Self.CenterX;
				}

				if (!tweened && T >= delay - 0.4f) {
					tweened = true;
					Self.GetComponent<MobAnimationComponent>().Animate();
				}
				
				if (T >= delay && Self.CanSeeTarget()) {
					var p = new ProjectilePattern(KeepShapePattern.Make(Rnd.Chance() ? -4 : 4)) {
						Position = Self.Center
					};
					
					Self.Area.Add(p);
					var sa = Rnd.AnglePI();
					var count = 5;

					for (var i = 0; i < count; i++) {
						var i1 = i;

						Timer.Add(() => {
							var a = (float) Math.PI * i1 / (count * 0.5f) + sa;
							var pr = Projectile.Make(Self, "small");

							pr.CanBeReflected = false;
							pr.Color = Self.color;
							pr.AddLight(32, Self.color);
							
							pr.CanBeReflected = false;
							pr.BodyComponent.Angle = a;

							pr.Center = Self.Center + MathUtils.CreateVector(a, 12);
							
							p.Add(pr);

							if (i1 == count - 1) {
								p.Launch(Self.Target == null ? Rnd.AnglePI() : Self.AngleTo(Self.Target), 80);
							}
						}, i * 0.1f);
					}

					Become<TeleportState>();
				}
			}
		}

		public class TeleportState : SmartState<TeleportingMage> {
			private bool teleported;

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 1.5f && !teleported) {
					teleported = true;

					var a = Self.GetComponent<MobAnimationComponent>();

					Tween.To(0.2f, a.Scale.X, x => a.Scale.X = x, 0.5f, Ease.QuadIn);
					Tween.To(2f, a.Scale.Y, x => a.Scale.Y = x, 0.5f, Ease.QuadIn).OnEnd = () => {
						var r = Self.GetComponent<RoomComponent>().Room;
						Vector2 s;

						do {
							s = r.GetRandomFreeTile() * 16 + new Vector2(8);
						} while (Self.Target != null && Self.Target.DistanceTo(s) < 32);
						
						Self.Center = s;

						Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.5f);
						Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.5f).OnEnd = () => {
							Become<IdleState>();
						};
					};
				}
			}
		}
		#endregion
	}
}