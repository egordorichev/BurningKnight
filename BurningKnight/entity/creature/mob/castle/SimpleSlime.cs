using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;

namespace BurningKnight.entity.creature.mob.castle {
	public class SimpleSlime : Slime {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("slime"));
			SetMaxHp(2);

			var body = new RectBodyComponent(2, 7, 12, 9);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
		}
	}
}