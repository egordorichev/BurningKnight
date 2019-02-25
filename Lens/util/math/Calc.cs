namespace Lens.util.math {
	public static class Calc {
		public static float Clamp(float value, float min, float max) {
			return System.Math.Max(min, System.Math.Min(value, max));
		}

		public static bool IsBitSet(byte b, int pos) {
			return (b & (1 << pos)) != 0;
		}
	}
}