using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.desert {
	public class Worm : Mob {
		protected override void SetStats() {
			base.SetStats();

			SetMaxHp(1);
			AddAnimation("worm");

			GetComponent<MobAnimationComponent>().ShadowOffset = 1;

			var body = new RectBodyComponent(3, 3, 4, 10);
			AddComponent(body);
			body.KnockbackModifier = 0;

			Become<IdleState>();
		}

		#region Worm 
		public class HiddenState : SmartState<Worm> {
			private float delay;
			private Vector2 target;

			public override void Destroy() {
				base.Destroy();

				var i = 0;

				do {
					var spot = Self.GetComponent<RoomComponent>().Room.GetRandomFreeTile() * 16;

					if (Self.Target == null || Self.Target.DistanceTo(spot) > 32f) {
						target = new Vector2(spot.X + 8, spot.Y);
						break;
					}

					i++;

					if (i > 99) {
						Log.Error("Failed to find a spot where to dig up");

						break;
					}
				} while (true);
				
				Self.TouchDamage = 0;
				Self.GetComponent<HealthComponent>().Unhittable = true;
				delay = Rnd.Float(0.5f, 1.5f);

				Self.Center = target;
				
				Self.TouchDamage = 1;
				Self.GetComponent<HealthComponent>().Unhittable = false;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<ShowingState>();
				}
			}
		}

		public class HidingState : SmartState<Worm> {
			public override void Init() {
				base.Init();
				Self.GetComponent<MobAnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<MobAnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<MobAnimationComponent>().Animation.Paused) {
					Become<HiddenState>();
				}
			}
		}

		public class ShowingState : SmartState<Worm> {
			public override void Init() {
				base.Init();
				Self.GetComponent<MobAnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<MobAnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (Self.GetComponent<MobAnimationComponent>().Animation.Paused) {
					Become<IdleState>();
				}

				Self.TurnToTarget();
			}
		}

		public class IdleState : SmartState<Worm> {
			public override void Update(float dt) {
				base.Update(dt);

				if (T >= 3f) {
					if (Self.Target == null || Self.DistanceTo(Self.Target) > 96f) {
						Become<HidingState>();
					} else {
						Become<FireState>();
					}
				}

				Self.TurnToTarget();
			}
		}

		public class FireState : SmartState<Worm> {
			private bool fired;
			
			public override void Update(float dt) {
				base.Update(dt);
				
				if (!fired && T >= 0.1f) {
					fired = true;

					if (Self.Target == null) {
						return;
					}

					var a = Self.GetComponent<MobAnimationComponent>();
					
					Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {

						Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
						Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

							Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
							Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f).OnEnd = () => {
								Become<HidingState>();
							};

							if (Self.Target == null) {
								return;
							}
								
							var ac = 0.1f;
							var angle = Self.AngleTo(Self.Target) + Rnd.Float(-ac, ac);
							var projectile = Projectile.Make(Self, "small", angle, 8f);

							projectile.Center += MathUtils.CreateVector(angle, 2f);
							projectile.AddLight(32f, Projectile.RedLight);

							AnimationUtil.Poof(projectile.Center);
						};
					};
				}

				Self.TurnToTarget();
			}
		}
		#endregion

		protected override void RenderShadow() {
			if (GetComponent<StateComponent>().StateInstance is HiddenState) {
				return;
			}
			
			base.RenderShadow();
		}

		public override void Render() {
			if (GetComponent<StateComponent>().StateInstance is HiddenState) {
				return;
			}
			
			base.Render();
		}
	}
}