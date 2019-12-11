using BurningKnight.entity.component;

namespace BurningKnight.entity.creature.mob.jungle {
	public class Bee : Mob {
		protected virtual string GetAnimation() {
			return "bee";
		}
		
		protected override void SetStats() {
			base.SetStats();

			Height = 12;
			
			AddAnimation(GetAnimation());
			SetMaxHp(5);
			
			Become<IdleState>();
			Flying = true;

			GetComponent<MobAnimationComponent>().ShadowOffset = -2;

			var body = new RectBodyComponent(2, 9, 12, 1);
			AddComponent(body);
			
			body.Body.LinearDamping = 4;
			body.Body.Restitution = 1;
			body.Body.Friction = 0;

			AddComponent(new SensorBodyComponent(3, 4, 10, 6));
		}

		public class IdleState : SmartState<Bee> {
			
		}
	}
}