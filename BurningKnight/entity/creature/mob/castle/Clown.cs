using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.castle {
	public class Clown : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("clown");
			SetMaxHp(2);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 6;
		}
		
		#region Clown States
		public class IdleState : MobState<Clown> {
			public override void Update(float dt) {
				base.Update(dt);

				if (Self.Target != null) {
					Self.MoveTo(Self.Target.Center, 30f);
				}
			}
		}
		#endregion
	}
}