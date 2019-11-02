using BurningKnight.entity.component;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.desert {
	public class Mummy : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			SetMaxHp(4);
			AddAnimation("mummy");
			
			var body = new RectBodyComponent(3, 3, 7, 13);
			AddComponent(body);

			body.KnockbackModifier = 3;
			body.Body.LinearDamping = 4;
			
			Become<IdleState>();
		}
		
		#region Mummy States
		public class IdleState : SmartState<Mummy> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null && Self.DistanceTo(Self.Target) < 64f) {
					Become<RunState>();
				}
			}
		}

		public class RunState : SmartState<Mummy> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target == null) {
					Self.Become<IdleState>();

					return;
				}

				var d = Self.DistanceTo(Self.Target);

				if (d > 96f) {
					Self.Become<IdleState>();
					return;
				}

				var dx = Self.DxTo(Self.Target);
				var dy = Self.DyTo(Self.Target);

				var s = dt * 300;

				Self.GetComponent<RectBodyComponent>().Velocity += new Vector2(dx / d * s, dy / d * s);
				Self.PushFromOtherEnemies(dt);
			}
		}
		#endregion
	}
}