﻿using System;
using System.Collections.Generic;
using System.Text;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace Lens.util.math {
	public static class Rnd {
		private static System.Random random = new System.Random(Guid.NewGuid().GetHashCode());
		private static string seed;
		private static int intSeed;

		public static System.Random Generator => random;
		
		public static string Seed {
			get => seed;

			set {
				seed = value;
				intSeed = ParseSeed(seed);
				random = new Random(intSeed);
			}
		}

		public static int IntSeed => intSeed;
		public static string SeedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
		
		static Rnd() {
			Seed = GenerateSeed();
			Log.Debug($"Random seed is {seed}");
		}
		
		public static int ParseSeed(string seed) {
			if (seed == null) {
				return 0;
			}
			
			var value = 0;
			var i = 0;

			foreach (var c in seed) {
				var index = SeedChars.IndexOf(c);

				if (index == -1) {
					Log.Error($"Unknown seed char '{c}' ({(int) c})!");
					continue;
				}

				value += index << (i * 4);
				i++;
			}
			
			return value;
		}

		public static string GenerateSeed(int len = 8, int seed = -1) {
			var builder = new StringBuilder();
			var r = seed == -1 ? new Random(Environment.TickCount + Guid.NewGuid().GetHashCode()) : new Random(seed);

			for (var i = 0; i < len; i++) {
				builder.Append(SeedChars[r.Next(SeedChars.Length - 1)]);
			}
			
			return builder.ToString();
		}

		public static Vector2 Vector(float min, float max) {
			if (min > max) {
				var t = min;
				
				min = max;
				max = t;
			}
			
			return new Vector2(Float(min, max), Float(min, max));
		}

		public static int Int() {
			return random.Next();
		}
		
		public static int Int(int max) {
			return random.Next(0, max);
		}
		
		public static int Int(int min, int max) {
			if (min > max) {
				var t = min;
				
				min = max;
				max = t;
			}
		
			return random.Next(min, max);
		}

		public static int IntCentred(int min, int max) {
			if (min > max) {
				var t = min;
				
				min = max;
				max = t;
			}
			
			return (int) ((Int(min, max) + Int(min, max)) / 2f - 0.1f);
		}

		public static float Float() {
			return (float) random.NextDouble();
		}
		
		public static float Float(float max) {
			return (float) (random.NextDouble() * max);
		}

		public static float Float(float min, float max) {
			if (min > max) {
				var t = min;
				
				min = max;
				max = t;
			}
			
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

		public static T Element<T>(List<T> list, Func<T, bool> filter) where T : Entity {
			var length = list.Count;
			var sum = 0;

			foreach (var e in list) {
				if (filter(e)) {
					sum++;
				}
			}

			int value = Int(sum);
			sum = 0;

			for (int i = 0; i < length; i++) {
				if (filter(list[i])) {
					sum++;
					
					if (value < sum) {
						return list[i];
					}
				}
			}

			return null;
		}
	}
}