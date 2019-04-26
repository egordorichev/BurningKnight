using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class BabySlime : Slime {
		private static readonly Color color = ColorUtils.FromHex("#0069aa");
		
		protected override Color GetColor() {
			return color;
		}

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