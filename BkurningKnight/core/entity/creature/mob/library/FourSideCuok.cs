using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.library {
	public class FourSideCuok : Cuok {
		public static Animation Animations = Animation.Make("actor-cuok", "-cyan");

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void SpawnBullet(float A) {
			for (int I = 0; I < 4; I++) {
				base.SpawnBullet((float) (A + Math.PI / 2 * I));
			}
		}

		public override int GetMax() {
			return 3;
		}
	}
}
