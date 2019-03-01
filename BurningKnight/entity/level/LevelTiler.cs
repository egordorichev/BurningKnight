using BurningKnight.util;
using Lens.util.math;

namespace BurningKnight.entity.level {
	public class LevelTiler {
		public static void TileUp(Level level) {
			for (int y = 0; y < level.Height; y++) {
				for (int x = 0; x < level.Width; x++) {
					TileUp(level, level.ToIndex(x, y));
				}
			}
		}

		public static void TileUp(Level level, int index) {
			var tile = level.Tiles[index];
			var t = (Tile) tile;

			if (t.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD)) {
				level.Variants[index] = (byte) Random.Int(12); // todo: big chunks
				return;
			}

			byte mask = 0;
			
			for (int i = 0; i < 4; i++) {
				if (ShouldTile(tile, level.Tiles[index + PathFinder.Circle4[i]])) {
					mask += (byte) (1 << i);
				}
			}

			level.Variants[index] = mask;
		}

		public static bool ShouldTile(byte tile, byte to) {
			return tile == to; // Gets much more complex, but later
		}
	}
}