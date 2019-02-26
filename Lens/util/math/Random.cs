using System;
using System.Collections.Generic;

namespace Lens.util.math {
	public static class Random {
		private static System.Random random = new System.Random();

		public static int Int() {
			return random.Next();
		}
		
		public static int Int(int max) {
			return random.Next(0, max);
		}
		
		public static int Int(int min, int max) {
			return random.Next(min, max);
		}

		public static float Float() {
			return (float) random.NextDouble();
		}
		
		public static float Float(float max) {
			return (float) (random.NextDouble() * max);
		}

		public static float Float(float min, float max) {
			return (float) (random.NextDouble() * (max - min) + min);
		}

		public static double Double() {
			return random.NextDouble();
		}

		public static double Double(double min, double max) {
			return random.NextDouble() * (max - min) + min;
		}

		public static bool Bool() {
			return random.NextDouble() >= 0.5;
		}

		public static bool Chance(float chance) {
			return random.NextDouble() * 100 >= chance;
		}

		public static float Angle() {
			return Float(360);
		}

		public static float AnglePI() {
			return Float((float) (Math.PI * 2));
		}

		public static int Chances(List<float> chances) {
			var length = chances.Count;
			float sum = 0;

			foreach (var chance in chances) {
				sum += chance;
			}

			float value = Float(sum);
			sum = 0;

			for (int i = 0; i < length; i++) {
				sum += chances[i];

				if (value < sum) {
					return i;
				}
			}

			return -1;
		}
	}
}