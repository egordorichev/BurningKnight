using BurningKnight.core.util;

namespace BurningKnight.core.entity.level {
	public class Patch {
		public static bool Generate(float Seed, int Octaves) {
			return Generate(Level.GetWidth(), Level.GetHeight(), Seed, Octaves);
		}

		public static bool Generate(int W, int H, float Seed, int Octaves) {
			bool[] Cur = new bool[W * H];
			bool[] Off = new bool[W * H];

			for (int I = 0; I < W * H; I++) {
				Off[I] = Random.NewFloat() < Seed;
			}

			for (int I = 0; I < Octaves; I++) {
				for (int Y = 1; Y < H - 1; Y++) {
					for (int X = 1; X < W - 1; X++) {
						int Pos = X + Y * W;
						int Count = 0;

						if (Off[Pos - W - 1]) {
							Count++;
						} 

						if (Off[Pos - W]) {
							Count++;
						} 

						if (Off[Pos - W + 1]) {
							Count++;
						} 

						if (Off[Pos - 1]) {
							Count++;
						} 

						if (Off[Pos + 1]) {
							Count++;
						} 

						if (Off[Pos + W - 1]) {
							Count++;
						} 

						if (Off[Pos + W]) {
							Count++;
						} 

						if (Off[Pos + W + 1]) {
							Count++;
						} 

						if (!Off[Pos] && Count >= 5) {
							Cur[Pos] = true;
						} else {
							Cur[Pos] = Off[Pos] && Count >= 4;
						}

					}
				}

				bool[] Tmp = Cur;
				Cur = Off;
				Off = Tmp;
			}

			return Off;
		}
	}
}
