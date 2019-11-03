using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using BurningKnight.entity.projectile;
using BurningKnight.util;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.desert {
	public class Maggot : WallWalker {
		private const float TargetDistance = 4f;
		private const float Speed = 30f;
		
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new WallAnimationComponent("maggot"));
			SetMaxHp(1);

			Depth = Layers.Creature + 1;
		}
        	
		#region Crawler States
		public class IdleState : WallWalker.IdleState {
			private bool stop;
			
			public override void DoLogic(float dt) {
				base.DoLogic(dt);

				if (Self.Target == null) {
					ResetVelocity();
					return;
				}

				if (mx == 0) {
					if (Math.Abs(Self.DxTo(Self.Target)) <= TargetDistance) {
						ResetVelocity();
						Become<FireState>();

						return;
					}

					var left = Self.CenterX > Self.Target.CenterX;

					if (Self.Left != left) {
						InvertVelocity();
						stop = false;
					} else {
						vx = Self.Left ? -1 : 1;
						vy = 0;	
					}
				} else {
					if (Math.Abs(Self.DyTo(Self.Target)) <= TargetDistance) {
						ResetVelocity();
						Become<FireState>();

						return;
					}

					var left = Self.CenterY > Self.Target.CenterY;

					if (Self.Left != left) {
						InvertVelocity();
						stop = false;
					} else {
						vx = 0;
						vy = Self.Left ? -1 : 1;		
					}
				}

				if (stop) {
					vx = 0;
					vy = 0;
				}
				
				velocity = new Vector2(vx * Speed, vy * Speed);
			}
			
			public override void Flip() {
				ResetVelocity();
				stop = true;
			}
		}

		public class FireState : SmartState<Maggot> {
			private bool shot; 
			
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

				if (!shot && Self.GetComponent<WallAnimationComponent>().Animation.Paused) {
					if (Self.Target == null) {
						Become<IdleState>();
						return;
					}

					shot = true;

					var a = Self.GetComponent<WallAnimationComponent>();

					var angle = Self.Direction.ToAngle();
					var projectile = Projectile.Make(Self, "small", angle, 5f);

					projectile.AddLight(32f, Projectile.RedLight);
					projectile.Center += MathUtils.CreateVector(angle, 4);
							
					AnimationUtil.Poof(projectile.Center);

					a.Scale.X = 1.8f;
					a.Scale.Y = 0.2f;
					
					Tween.To(1, a.Scale.X, x => a.Scale.X = x, 0.4f);
					Tween.To(1, a.Scale.Y, x => a.Scale.Y = x, 0.4f).OnEnd = () => { Become<IdleState>(); };
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