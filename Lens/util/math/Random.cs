using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Lens.util.math {
	public static class Random {
		private static System.Random random = new System.Random();
		private static string seed;

		public static System.Random Generator => random;
		
		public static string Seed {
			get => seed;

			set {
				seed = value;
				random = new System.Random(ParseSeed(seed));
			}
		}

		public static string SeedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
		
		static Random() {
			Seed = GenerateSeed();
			Log.Debug($"Random seed is {seed}");
		}
		
		public static int ParseSeed(string seed) {
			if (seed == null) {
				return 0;
			}
			
			int value = 0;

			foreach (var c in seed) {
				value += SeedChars.IndexOf(c) * SeedChars.Length;
			}

			return value;
		}

		public static string GenerateSeed(int len = 8) {
			var builder = new StringBuilder();
			var r = new System.Random();

			for (int i = 0; i < len; i++) {
				builder.Append(SeedChars[r.Next(SeedChars.Length - 1)]);
			}
			
			return builder.ToString();
		}

		public static Vector2 Vector(float min, float max) {
			return new Vector2(Float(min, max), Float(min, max));
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

		public static int IntCentred(int min, int max) {
			return (int) ((Int(min, max) + Int(min, max)) / 2f - 0.1f);
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

		public static Vector2 Offset(float d) {
			var a = AnglePI();
			return new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
		}
		
		public static double Double() {
			return random.NextDouble();
		}
		
		public static double Double(double max) {
			return random.NextDouble() * max;
		}

		public static double Double(double min, double max) {
			return random.NextDouble() * (max - min) + min;
		}

		public static bool Bool() {
			return random.NextDouble() >= 0.5;
		}

		public static bool Chance(float chance = 50) {
			return random.NextDouble() * 100 <= chance;
		}

		public static float Angle() {
			return Float(360);
		}

		public static float AnglePI() {
			return Float((float) (Math.PI * 2));
		}

		public static int Chances(float[] chances) {
			var length = chances.Length;
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

		public static int Sign() {
			return Chance() ? -1 : 1;
		}
	}
}