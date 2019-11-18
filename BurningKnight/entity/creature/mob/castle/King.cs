using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.castle {
	public class King : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("king");
			SetMaxHp(3);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 4;
		}
		
		#region King States
		public class IdleState : SmartState<King> {
			
		}
		#endregion
	}
}