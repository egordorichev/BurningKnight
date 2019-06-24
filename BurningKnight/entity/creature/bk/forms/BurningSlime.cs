using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.bk.forms {
	public class BurningSlime : Slime {
		private static readonly Color color = ColorUtils.FromHex("#ff0040");
		
		protected override Color GetBloodColor() {
			return color;
		}
		
		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("bk_slime"));
			SetMaxHp(100);

			Width = 34;
			Height = 28;

			var body = new RectBodyComponent(5, 10, 24, 19);
			AddComponent(body);

			body.Body.LinearDamping = 1;
			body.KnockbackModifier = 0.25f;

			ZVelocity = 7;
			JumpForce = 200;
		}
	}
}