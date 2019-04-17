using System;
using BurningKnight.entity.component;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob.castle {
	public class Slime : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("slime");
			SetMaxHp(2);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 1;
		}
		
		#region Slime States
		public class IdleState : MobState<Slime> {
			private float delay;

			public override void Init() {
				base.Init();
				
				delay = Random.Float(0.9f, 1.2f);
				Self.GetComponent<RectBodyComponent>().Velocity = Vector2.Zero;
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<JumpState>();
				}
			}
		}

		public class JumpState : MobState<Slime> {
			private Vector2 velocity;
			private float delay;
			
			public override void Init() {
				base.Init();

				var a = Self.Target == null ? Random.AnglePI() : Self.AngleTo(Self.Target);
				var force = Random.Float(20f) + 140f;
				
				velocity = new Vector2((float) Math.Cos(a) * force, (float) Math.Sin(a) * force);
				Self.GetComponent<RectBodyComponent>().Velocity = velocity;
				
				delay = Random.Float(0.4f, 0.6f);
			}

			public override void Update(float dt) {
				base.Update(dt);

				if (T >= delay) {
					Become<IdleState>();
				}
			}
		}
		#endregion
	}
}