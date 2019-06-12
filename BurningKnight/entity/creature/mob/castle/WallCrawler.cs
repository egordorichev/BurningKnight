using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class WallCrawler : WallWalker {
		protected override void SetStats() {
			base.SetStats();
			
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

		public class FireState : CreatureState<WallCrawler> {
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

					if (Self.Target != null) {
						var angle = Self.Direction.ToAngle();
						var projectile = Projectile.Make(Self, "small", angle, 7f);

						projectile.AddLight(32f, Color.Red);
					}
				} else if (fired && T > 0.2f) {
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