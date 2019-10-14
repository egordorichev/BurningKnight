using System;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class WallCrawler : WallWalker {
		protected override void SetStats() {
			base.SetStats();
			
			GetComponent<BuffsComponent>().Immune.Add(typeof(FrozenBuff));
			AddComponent(new WallAnimationComponent("crawler"));
			SetMaxHp(2);
		}

		#region Crawler States
		public class IdleState : WallWalker.IdleState {
			public override void Update(float dt) {
				base.Update(dt);
				
				if (T >= 3f) {
					Become<FireState>();
				}
			}
			
			public override void Flip() {
				Self.Left = !Self.Left;

				if (Self.T >= 3f) {
					Become<FireState>();
					return;
				}

				velocity *= -1;
				vx *= -1;
				vy *= -1;
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				T = 0;
			}
		}

		public class FireState : SmartState<WallCrawler> {
			private bool fired;
			
			public override void Init() {
				base.Init();

				Self.T = 0;
				
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
				Self.GetComponent<WallAnimationComponent>().SetAutoStop(true);
			}

			public override void Destroy() {
				base.Destroy();
				Self.GetComponent<WallAnimationComponent>().SetAutoStop(false);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (!fired && Self.GetComponent<WallAnimationComponent>().Animation.Paused) {
					fired = true;
					T = 0;

					if (Self.Target == null) {
						return;
					}

					var a = Self.GetComponent<WallAnimationComponent>();

					Tween.To(0.6f, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(1.6f, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {

						Tween.To(1.8f, a.Scale.X, x => a.Scale.X = x, 0.1f);
						Tween.To(0.2f, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {

							Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
							Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f);

							if (Self.Target == null) {
								return;
							}

							var angle = Self.Direction.ToAngle();
							var projectile = Projectile.Make(Self, "small", angle, 5f);

							projectile.AddLight(32f, Projectile.RedLight);
							projectile.Center += MathUtils.CreateVector(angle, 8);
							
							AnimationUtil.Poof(projectile.Center);
						};
					};
				} else if (fired && T > 1f) {
					Self.GetComponent<StateComponent>().Become<IdleState>();
				}
			}
		}
		#endregion

		protected override Type GetIdleState() {
			return typeof(IdleState);
		}

		public override bool SpawnsNearWall() {
			return true;
		}

		public override void Update(float dt) {
			base.Update(dt);
			T += dt;
		}
	}
}