using BurningKnight.state;
using Lens.util.math;

namespace BurningKnight.level {
	public class Patch {
		public static bool[] Generate(float Seed, int Octaves) {
			return Generate(Run.Level.Width, Run.Level.Height, Seed, Octaves);
		}

		public static bool[] Generate(int W, int H, float Seed, int Octaves) {
			var Cur = new bool[W * H];
			var Off = new bool[W * H];

			for (var I = 0; I < W * H; I++) Off[I] = Random.Float() < Seed;

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