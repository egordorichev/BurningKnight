using BurningKnight.state;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level {
	public class Patch {
		public static bool[] Generate(float Seed, int Octaves) {
			return Generate(Run.Level.Width, Run.Level.Height, Seed, Octaves);
		}

		public static bool[] Noise(float seed, float s = 0.1f) {
			return Noise(Run.Level.Width, Run.Level.Height, seed, s);
		}

		public static bool[] Noise(int w, int h, float seed, float s) {
			seed = 1 - seed;
			seed *= 2;
			seed -= 1;

			var mx = Rnd.Float(100f);
			var my = Rnd.Float(100f);
			var array = new bool[w * h];
			
			for (int y = 0; y < h; y++) {
				for (int x = 0; x < w; x++) {
					var n = Lens.util.Noise.Fbm(new Vector2(x * s + mx, y * s + my), 3);
					array[x + y * w] = n >= seed;
				}
			}

			return array;
		}

		public static bool[] GenerateWithNoise(int W, int H, float Seed, float h, float scale = 1f) {
			var a = new bool[W * H];

			for (var y = 0; y < H; y++) {
				for (var x = 0; x < W; x++) {
					a[x + y * W] = Lens.util.Noise.Generate((x + Seed) * scale, (y + Seed) * scale) > h;
				}
			}
			
			return a;
		}

		public static bool[] Generate(int W, int H, float Seed, int Octaves) {
			var Cur = new bool[W * H];
			var Off = new bool[W * H];

			for (var I = 0; I < W * H; I++) Off[I] = Rnd.Float() < Seed;

			for (var I = 0; I < Octaves; I++) {
				for (var Y = 1; Y < H - 1; Y++)
				for (var X = 1; X < W - 1; X++) {
					var Pos = X + Y * W;
					var Count = 0;

					if (Off[Pos - W - 1]) Count++;

					if (Off[Pos - W]) Count++;

					if (Off[Pos - W + 1]) Count++;

					if (Off[Pos - 1]) Count++;

					if (Off[Pos + 1]) Count++;

					if (Off[Pos + W - 1]) Count++;

					if (Off[Pos + W]) Count++;

					if (Off[Pos + W + 1]) Count++;

					if (!Off[Pos] && Count >= 5)
						Cur[Pos] = true;
					else
						Cur[Pos] = Off[Pos] && Count >= 4;
				}

				var Tmp = Cur;
				Cur = Off;
				Off = Tmp;
			}

			return Off;
		}
	}
}