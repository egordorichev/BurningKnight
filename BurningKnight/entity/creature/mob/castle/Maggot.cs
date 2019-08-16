using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class Maggot : WallWalker {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new WallAnimationComponent("maggot"));
			SetMaxHp(1);
		}

		protected override float GetSpeed() {
			return 45;
		}
		
		#region Maggot States
		public class IdleState : WallWalker.IdleState {
			public override void Update(float dt) {
				base.Update(dt);
				
				if (T >= 3f) {
					Become<WaitState>();
				}
			}
			
			public override void Flip() {
				Self.Left = !Self.Left;

				if (Self.T >= 3f) {
					Become<WaitState>();
					return;
				}

				velocity *= -1;
				vx *= -1;
				vy *= -1;
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				T = 0;
			}
		}

		public class WaitState : SmartState<Maggot> {
			private float time;

			public override void Init() {
				base.Init();
				
				time = Random.Float(1, 2f);
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= time) {
					Become<IdleState>();
				}
			}
		}
		#endregion

		protected override Type GetIdleState() {
			return typeof(IdleState);
		}

		public override float GetWeight() {
			return 0.5f;
		}
	}
}