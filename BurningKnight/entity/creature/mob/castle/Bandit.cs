using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.castle {
	public class Bandit : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("bandit");
			SetMaxHp(1);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 6;
		}
		
		#region Bandit States
		public class IdleState : MobState<Clown> {
			
		}
		#endregion
	}
}