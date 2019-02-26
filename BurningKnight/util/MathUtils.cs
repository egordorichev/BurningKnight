using System;

namespace BurningKnight.util {
	public class MathUtils {
		public static float Clamp(float Min, float Max, float Val) {
			return Math.Max(Min, Math.Min(Max, Val));
		}

		public static float Map(float X, float In_min, float In_max, float Out_min, float Out_max) {
			return (X - In_min) * (Out_max - Out_min) / (In_max - In_min) + Out_min;
		}
	}
}