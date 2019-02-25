using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.library {
	public class FourSideCuok : Cuok {
		public static Animation Animations = Animation.Make("actor-cuok", "-cyan");

		public Animation GetAnimation() {
			return Animations;
		}

		public override void SpawnBullet(float A) {
			for (var I = 0; I < 4; I++) base.SpawnBullet(A + Math.PI / 2 * I);
		}

		public override int GetMax() {
			return 3;
		}
	}
}