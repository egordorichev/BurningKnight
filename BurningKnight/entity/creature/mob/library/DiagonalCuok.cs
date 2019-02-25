using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.library {
	public class DiagonalCuok : Cuok {
		public static Animation Animations = Animation.Make("actor-cuok", "-yellow");

		public Animation GetAnimation() {
			return Animations;
		}

		public override float CalcAngle(float A, int Num) {
			return (float) Math.ToRadians(Math.Round((A - 45f) / 90f) * 90f + 45f);
		}
	}
}