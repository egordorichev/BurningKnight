using System;
using System.Collections.Generic;
using System.Text;

namespace Lens.util.math {
	public static class Random {
		private static System.Random random = new System.Random();
		private static string seed;
		
		public static string Seed {
			get => seed;

			set {
				seed = value;
				random = new System.Random(ParseSeed(seed));
			}
		}

		private static string seedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		
		static Random() {
			Seed = GenerateSeed();
		}
		
		public static int ParseSeed(string seed) {
			int value = 0;

			foreach (var c in seed) {
				value += seedChars.IndexOf(c);
			}

			return value;
		}

		public static string GenerateSeed(int len = 8) {
			var builder = new StringBuilder();

			for (int i = 0; i < len; i++) {
				builder.Append(seedChars[Int(seedChars.Length)]);
			}
			
			return builder.ToString();
		}

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