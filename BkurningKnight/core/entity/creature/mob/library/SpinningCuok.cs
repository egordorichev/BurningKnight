using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.library {
	public class SpinningCuok : Cuok {
		protected void _Init() {
			{
				HpMax = 8;
			}
		}

		public static Animation Animations = Animation.Make("actor-cuok", "-green");

		public Animation GetAnimation() {
			return Animations;
		}

		public override float CalcAngle(float A, int Num) {
			return (float) (Math.ToRadians(Math.Round((A - 45f) / 90f) * 90f + 45f) + Num * Math.PI / 8);
		}

		public override bool Recalc() {
			return true;
		}

		public override int GetMax() {
			return 16;
		}

		public SpinningCuok() {
			_Init();
		}
	}
}
