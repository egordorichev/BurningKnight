using System;
using Lens.util;
using Microsoft.Xna.Framework;

namespace Lens.graphics {
	public static class ColorUtils {
		public static Vector4 White = new Vector4(1, 1, 1, 1);
		public static Vector4 Black = new Vector4(0, 0, 0, 1);
		public static Vector4 HalfBlack = new Vector4(0, 0, 0, 0.5f);
		public static Color WhiteColor = Color.White;
		public static Color HalfWhiteColor = new Color(1f, 1f, 1f, 0.5f);
		public static Color GrayColor = new Color(0.5f, 0.5f, 0.5f, 1f);
		public static Color BlackColor = Color.Black;
		
		public static Color FromHex(string hex) {
			Color color = new Color();

			color.R = (byte) int.Parse(hex.Substring(1, 2), System.Globalization.NumberStyles.HexNumber);
			color.G = (byte) int.Parse(hex.Substring(3, 2), System.Globalization.NumberStyles.HexNumber);
			color.B = (byte) int.Parse(hex.Substring(5, 2), System.Globalization.NumberStyles.HexNumber);
			color.A = 255;
			
			return color;
		}

		public static bool Compare(byte a, byte b, byte ac) {
			return Math.Abs(a - b) <= ac;
		}
		
		public static bool Compare(Color a, Color b, byte ac) {
			return Compare(a.R, b.R, ac) && Compare(a.G, b.G, ac) && Compare(a.B, b.B, ac);
		}

		public static Color FromHSV(float H, float S, float V) {
			double R;
			double G;
			double B;
			int I;
			float F;
			float P;
			float Q;
			float T;
			H = (float) Math.Max(0.0, Math.Min(360.0, H));
			S = (float) Math.Max(0.0, Math.Min(100.0, S));
			V = (float) Math.Max(0.0, Math.Min(100.0, V));
			S /= 100;
			V /= 100;
			H /= 60;
			I = (int) Math.Floor(H);
			F = H - I;
			P = V * (1 - S);
			Q = V * (1 - S * F);
			T = V * (1 - S * (1 - F));

			switch (I) {
				case 0: {
					R = Math.Round(255 * V);
					G = Math.Round(255 * T);
					B = Math.Round(255 * P);

					break;
				}

				case 1: {
					R = Math.Round(255 * Q);
					G = Math.Round(255 * V);
					B = Math.Round(255 * P);

					break;
				}

				case 2: {
					R = Math.Round(255 * P);
					G = Math.Round(255 * V);
					B = Math.Round(255 * T);

					break;
				}

				case 3: {
					R = Math.Round(255 * P);
					G = Math.Round(255 * Q);
					B = Math.Round(255 * V);

					break;
				}

				case 4: {
					R = Math.Round(255 * T);
					G = Math.Round(255 * P);
					B = Math.Round(255 * V);

					break;
				}

				default: {
					R = Math.Round(255 * V);
					G = Math.Round(255 * P);
					B = Math.Round(255 * Q);

					break;
				}
			}

			return new Color((float) R / 255.0f, (float) G / 255.0f, (float) B / 255.0f, 1);
		}
		
		public static Color Mod(Color c) {
			var color = new Color();

			var f = 30;

			color.R = (byte) MathUtils.Clamp(0, 255, c.R + util.math.Random.Int(-f, f));
			color.G = (byte) MathUtils.Clamp(0, 255, c.G + util.math.Random.Int(-f, f));
			color.B = (byte) MathUtils.Clamp(0, 255, c.B + util.math.Random.Int(-f, f));
			color.A = 255;

			return color;
		}
	}
}