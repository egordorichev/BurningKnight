using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.library {
	public class Skeleton : Mob {
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
			AddComponent(new MobAnimationComponent("skeleton"));
			
			Become<IdleState>();
		}

		#region Skeleton States
		public class IdleState : SmartState<Skeleton> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null) {
					Self.MoveTo(Self.Target.Center, 80f, 8f, true);
				}
			}
		}
		#endregion
	}
}