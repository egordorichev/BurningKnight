using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;

namespace BurningKnight.entity.creature.mob.castle {
	public class BabySlime : Slime {
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("baby_slime"));
			SetMaxHp(1);

			var body = new RectBodyComponent(4, 9, 8, 7);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
		}
	}
}