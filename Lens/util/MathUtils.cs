using System;
using Microsoft.Xna.Framework;

namespace Lens.util {
	public class MathUtils {
		public static Vector2 InvertX = new Vector2(1, -1);
		public static Vector2 InvertY = new Vector2(1, -1);
		public static Vector2 InvertXY = new Vector2(-1, -1);
		public static Vector2 DoubleScale = new Vector2(2);
		
		public static Vector2[] Directions = {
			new Vector2(-1, 0),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(0, -1)
		};
		
		public static Vector2[] EntityDirections = {
			new Vector2(-1, 0),
			new Vector2(1, 0),
			new Vector2(0, -1)
		};
		
		public static float Clamp(float Min, float Max, float Val) {
			return Math.Max(Min, Math.Min(Max, Val));
		}

		public static float Map(float X, float In_min, float In_max, float Out_min, float Out_max) {
			return (X - In_min) * (Out_max - Out_min) / (In_max - In_min) + Out_min;
		}

		public static double LerpAngle(double a, double to, float dt) {
			return a + ShortAngleDistance(a, to) * dt;
		}
		
		public static double ShortAngleDistance(double a0, double a1) {
			double max = Math.PI * 2;
			double da = (a1 - a0) % max;
			return 2 * da % max - da;
		}

		public static Vector2 RotateAround(Vector2 point, float angle, Vector2 origin) {
			var s = Math.Sin(angle);
			var c = Math.Cos(angle);

			var p = point - origin;

			p.X = (float) (point.X * c - point.Y * s);
			p.Y = (float) (point.X * s + point.Y * c);
			
			return p + origin;
		}

		public static string ToRoman(int number) {
			if (number >= 50) {
				return "L" + ToRoman(number - 50);
			}
			
			if (number >= 40) {
				return "XL" + ToRoman(number - 40);
			}
			
			if (number >= 10) {
				return "X" + ToRoman(number - 10);
			}
			
			if (number >= 9) {
				return "IX" + ToRoman(number - 9);
			}
			
			if (number >= 5) {
				return "V" + ToRoman(number - 5);
			}
			
			if (number >= 4) {
				return "IV" + ToRoman(number - 4);
			}
			
			if (number > 1) {
				return "I" + ToRoman(number - 1);
			}

			if (number == 1) {
				return "I";
			}

			return "???";
		}
	}
}