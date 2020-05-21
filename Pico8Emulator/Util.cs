using System;
using System.Runtime.CompilerServices;

namespace Pico8Emulator {
	public static class Util {
		public const int Shift16 = 1 << 16;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static byte GetHalf(byte b, bool rightmost = true) {
			if (rightmost) {
				return (byte)(b & 0x0f);
			} else {
				return (byte)((b & 0xf0) >> 4);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void SetHalf(ref byte b, byte val, bool rightmost = true) {
			if (rightmost) {
				b = (byte)((byte)(b & 0xf0) | (val & 0x0f));
			} else {
				b = (byte)((byte)(b & 0x0f) | (val << 4));
			}
		}

		public static int FloatToFixed(double x) {
			return (int)(x * Shift16);
		}

		public static double FixedToFloat(int x) {
			return Math.Round((double)x / Shift16, 4);
		}

		public static void Swap<T>(ref T lhs, ref T rhs) {
			var temp = lhs;

			lhs = rhs;
			rhs = temp;
		}

		public static float NoteToFrequency(float note) {
			return (float)(440.0 * Math.Pow(2, (note - 33) / 12.0f));
		}

		public static float Lerp(float a, float b, float t) {
			return (b - a) * t + a;
		}
	}
}