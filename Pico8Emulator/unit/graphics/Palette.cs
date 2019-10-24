using System.Drawing;

namespace Pico8Emulator.unit.graphics {
	public static class Palette {
		public const int Size = 16;

		public static byte[,] standardPalette = {
			{ 0, 0, 0 },
			{ 29, 43, 83 },
			{ 126, 37, 83 },
			{ 0, 135, 81 },
			{ 171, 82, 54 },
			{ 95, 87, 79 },
			{ 194, 195, 199 },
			{ 255, 241, 232 },
			{ 255, 0, 77 },
			{ 255, 163, 0 },
			{ 255, 236, 39 },
			{ 0, 228, 54 },
			{ 41, 173, 255 },
			{ 131, 118, 156 },
			{ 255, 119, 168 },
			{ 255, 204, 170 }
		};

		public static byte ColorToPalette(Color col) {
			for (var i = 0; i < Size; i++) {
				if (standardPalette[i, 0] == col.R && standardPalette[i, 1] == col.G && standardPalette[i, 2] == col.B) {
					return (byte)i;
				}
			}

			return 0;
		}
	}
}