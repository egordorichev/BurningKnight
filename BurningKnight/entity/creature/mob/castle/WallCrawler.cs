using System;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class WallCrawler : WallWalker {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new WallAnimationComponent("crawler"));
			SetMaxHp(2);
		}	
		
		private static readonly Color color = ColorUtils.FromHex("#ffeb57");
		
		protected override Color GetBloodColor() {
			return color;
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

							Self.GetComponent<AudioEmitterComponent>().EmitRandomized("mob_fire_wall");
						
							var angle = Self.Direction.ToAngle();
							var builder = new ProjectileBuilder(Self, "small") {
								LightRadius = 32f
							};

							builder.AddFlags(ProjectileFlags.FlyOverStones);
							builder.Move(angle, 8);

							if (Run.Depth > 2 || Run.Loop > 0) {
								builder.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);
								builder.Scale *= 1.5f;
							}

							builder.Shoot(angle, 5f).Build();
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

		public override void Update(float dt) {
			base.Update(dt);
			T += dt;
		}
	}
}