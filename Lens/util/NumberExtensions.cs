using System;

namespace Lens.util {
	public static class NumberExtensions {
		public static double ToRadians(this double val) {
			return (Math.PI / 180) * val;
		}
		
		public static float ToRadians(this float val) {
			return (float) ((Math.PI / 180) * val);
		}

		public static double ToDegrees(this double val) {
			return Math.PI * val / 180.0;
		}
		
		public static float ToDegrees(this float val) {
			return (float) (Math.PI * val / 180.0);
		}
	}
}