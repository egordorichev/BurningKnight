using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.castle {
	public class BlueBulletSlime : BulletSlime {
		private static readonly Color color = ColorUtils.FromHex("#00cdf9");
		
		protected override Color GetBloodColor() {
			return color;
		}

		protected override void SetStats() {
			base.SetStats();
		}

		protected virtual string GetSprite() {
			return "blue_bullet_slime";
		}
	}
}