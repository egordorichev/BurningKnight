using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.library {
	public class SpinningCuok : Cuok {
		public static Animation Animations = Animation.Make("actor-cuok", "-green");

		public SpinningCuok() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 8;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override float CalcAngle(float A, int Num) {
			return Math.ToRadians(Math.Round((A - 45f) / 90f) * 90f + 45f) + Num * Math.PI / 8;
		}

		public override bool Recalc() {
			return true;
		}

		public override int GetMax() {
			return 16;
		}
	}
}