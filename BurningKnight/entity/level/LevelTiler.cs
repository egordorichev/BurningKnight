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
			var liquid = level.Liquid[index];
			byte lmask = 0;
			
			for (int i = 0; i < 4; i++) {
				var m = PathFinder.Circle4[i];
				var n = index + m;
				
				if (!level.IsInside(n) || ShouldTile(liquid, level.Tiles[n], level.Liquid[n])) {
					lmask |= (byte) (1 << i);
				}
			}

			level.LiquidVariants[index] = lmask;

			byte mask = 0;
			var tile = level.Tiles[index];
			var t = (Tile) tile;
			
			if (t.Matches(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD)) {
				if (level.Variants[index] != 0) {
					return;
				}

				var v = Random.Int(9);

				if (v == 8 || v == 9) {
					if (level.IsInside(index + level.Width + 1) && level.Tiles[index + 1] == tile && level.Tiles[index + level.Width] == tile && level.Tiles[index + 1 + level.Width] == tile 
					    && level.Variants[index + 1] == 0 && level.Variants[index + level.Width] == 0 && level.Variants[index + 1 + level.Width] == 0) {

						var st = v == 8 ? 8 : 10; 
						
						level.Variants[index] = (byte) st;
						level.Variants[index + 1] = (byte) (st + 1);
						level.Variants[index + level.Width] = (byte) (st + 4);
						level.Variants[index + 1 + level.Width] = (byte) (st + 5);
		
						return;
					}

					v = Random.Int(8);
				}

				level.Variants[index] = (byte) v;
				return;
			}

			if (!(t.Matches(TileFlags.LiquidLayer) || t.Matches(TileFlags.Solid))) {
				return;
			}
			
			for (int i = 0; i < 4; i++) {
				var m = PathFinder.Circle4[i];
				var n = index + m;
				
				if (!level.IsInside(n) || ShouldTile(tile, level.Tiles[n], level.Liquid[n])) {
					mask |= (byte) (1 << i);
				}
			}

			if (t.Matches(Tile.WallA, Tile.WallB)) {
				for (int i = 0; i < 4; i++) {
					var m = PathFinder.Corner[i];
					var n = index + m;
				
					if (!level.IsInside(n) || ShouldTile(tile, level.Tiles[n], level.Liquid[n])) {
						mask |= (byte) (1 << (4 + i));
					}
				}
			}
			
			level.Variants[index] = mask;
		}

		public static bool ShouldTile(byte tile, byte to, byte l) {
			var t = (Tile) tile;
			var tt = (Tile) to;
			var ll = (Tile) l;

			if (t.IsWall()) {
				return tt.IsWall();
			}

			if (t == Tile.Grass || t == Tile.HighGrass) {
				return ll == Tile.Grass || ll == Tile.HighGrass || tt.IsWall();
			}

			if (t.Matches(TileFlags.LiquidLayer)) {
				return tt.IsWall() || tt == Tile.Chasm || ll == t;
			}
			
			return tile == to || tile == l;
		}
	}
}