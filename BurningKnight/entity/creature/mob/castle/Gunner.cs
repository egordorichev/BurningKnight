using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.castle {
	public class Gunner : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			AddAnimation("gunner");
			SetMaxHp(3);
			
			Become<IdleState>();

			var body = new RectBodyComponent(2, 2, 12, 12);
			AddComponent(body);

			body.Body.LinearDamping = 6;
		}
		
		#region Clown States
		public class IdleState : MobState<Clown> {
			
		}
		#endregion
	}
}