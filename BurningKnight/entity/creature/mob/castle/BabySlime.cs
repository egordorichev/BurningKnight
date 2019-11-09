using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob.prefabs;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class BabySlime : Slime {
		private static readonly Color color = ColorUtils.FromHex("#0069aa");
		
		protected override Color GetBloodColor() {
			return color;
		}

		protected override float GetJumpDelay() {
			return -0.2f;
		}

		protected override void SetStats() {
			base.SetStats();
			
			AddComponent(new ZAnimationComponent("baby_slime"));
			SetMaxHp(1);

			var body = new RectBodyComponent(4, 9, 8, 7);
			AddComponent(body);

			body.Body.LinearDamping = 2;
			body.KnockbackModifier = 0.5f;
			
			AddComponent(new SensorBodyComponent(4, 15, 8, 1));
			
			AddComponent(new LightComponent(this, 32, MotherSlime.LightColor));
		}
	}
}