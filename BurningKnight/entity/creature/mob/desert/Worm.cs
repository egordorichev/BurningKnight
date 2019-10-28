using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.desert {
	public class Worm : Mob {
		protected override void SetStats() {
			base.SetStats();
			
			SetMaxHp(4);
			AddAnimation("worm");

			GetComponent<MobAnimationComponent>().ShadowOffset = 1;
			
			var body = new RectBodyComponent(3, 3, 4, 10);
			AddComponent(body);
			body.KnockbackModifier = 0;
			
			Become<IdleState>();
		}
		
		#region Worm States
		public class IdleState : SmartState<Worm> {
			
		}
		#endregion
	}
}