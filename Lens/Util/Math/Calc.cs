namespace Lens.Util.Math {
	public static class Calc {
		public static float Clamp(float value, float min, float max) {
			return System.Math.Max(min, System.Math.Min(value, max));
		}
	}
}