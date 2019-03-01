namespace BurningKnight.entity.level {
	public static class TileFlags {
		private static int[] flags = new int[(int) Tile.Total];
		
		public static int Passable = 0x1;
		public static int Solid = 0x2;
		public static int Hole = 0x4;
		public static int High = 0x8;
		public static int Burns = 0x10;
		public static int BreaksView = 0x20;
		public static int LiquidLayer = 0x40;

		static TileFlags() {
			flags[(int) Tile.Chasm] = Hole;
			flags[(int) Tile.Dirt] = Passable | LiquidLayer;
			flags[(int) Tile.Grass] = Passable | Burns | LiquidLayer;
			flags[(int) Tile.FloorA] = Passable;
			flags[(int) Tile.Wall] = Solid | High | BreaksView;
			flags[(int) Tile.Crack] = Solid | High | BreaksView;
			flags[(int) Tile.Water] = Passable | LiquidLayer;
			flags[(int) Tile.Venom] = LiquidLayer;
			flags[(int) Tile.WallSide] = 0;
			flags[(int) Tile.FloorB] = Passable | Burns;
			flags[(int) Tile.FloorC] = Passable;
			flags[(int) Tile.FloorD] = Passable;
			flags[(int) Tile.Lava] = LiquidLayer;
			flags[(int) Tile.Ice] = Passable | LiquidLayer;
			flags[(int) Tile.HighGrass] = Passable | Burns | LiquidLayer;
			flags[(int) Tile.Obsidian] = Passable | LiquidLayer;
			flags[(int) Tile.Ember] = Passable | LiquidLayer;
			flags[(int) Tile.Cobweb] = Passable | LiquidLayer | Burns;
		}
	
		public static bool Matches(this Tile tile, int flag) {
			return (flags[(int) tile] & flag) == flag;
		}
		
		public static bool Matches(byte tile, int flag) {
			return (flags[tile] & flag) == flag;
		}
	
		public static bool Matches(this Tile tile, params Tile[] tiles) {
			foreach (var type in tiles) {
				if (type == tile) {
					return true;
				}
			}

			return false;
		}		
	}
}