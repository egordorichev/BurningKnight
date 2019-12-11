using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.creature.mob.jungle {
	public class Sniper : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("sniper");
			SetMaxHp(5);
			
			Become<IdleState>();

			var body = new RectBodyComponent(3, 13, 10, 1);
			AddComponent(body);
			body.Body.LinearDamping = 10;

			AddComponent(new SensorBodyComponent(2, 2, 12, 12));
		}

		protected override bool HandleDeath(DiedEvent d) {
			ExplosionMaker.Make(this);
			return base.HandleDeath(d);
		}

		private int moveId;
		
		#region Bandit States
		public class IdleState : SmartState<Sniper> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 5f && Self.CanSeeTarget()) {
					Become<AimState>();
				}
			}
		}

		private float lastAngle;

		public class AimState : SmartState<Sniper> {
			private Vector2 lastSeen;
			
			public override void Init() {
				base.Init();
				
				Self.lastAngle = Self.AngleTo(Self.Target);
				lastSeen = Self.Target.Center;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T < 5f) {
					if (Self.CanSeeTarget()) {
						lastSeen = Self.Target.Center;
					}

					Self.lastAngle = (float) MathUtils.LerpAngle(Self.lastAngle, Self.AngleTo(lastSeen), dt * 2f);
				} else if (T >= 6f) {
					Become<IdleState>();
				}
			}

			public override void Destroy() {
				base.Destroy();
				
				if (Self.Target == null) {
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

						for (var i = 0; i < 3; i++) {
							var angle = Self.lastAngle + (i - 1) * 0.5f;
							var projectile = Projectile.Make(Self, "rect", angle, 20f);

							projectile.Center += MathUtils.CreateVector(angle, 8f);
							projectile.Spectral = true;
						}
					};
				};
			}
		}

		public override void Render() {
			base.Render();

			if (GetComponent<StateComponent>().StateInstance is AimState) {
				Graphics.Batch.DrawLine(Center, new Vector2((int) (Center.X + Math.Cos(lastAngle) * Display.UiWidth), 
					(int) (Center.Y + Math.Sin(lastAngle) * Display.UiWidth)), PlayerGraphicsComponent.AimLineColor, 1);
			}
		}
		#endregion

		protected override string GetDeadSfx() {
			return "mob_bandit_death";
		}
	}
}