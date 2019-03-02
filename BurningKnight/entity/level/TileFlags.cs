namespace BurningKnight.entity.level {
	public static class TileFlags {
		private static int[] flags = new int[(int) Tile.Total];
		
		public static int Passable = 0x1;
		public static int Solid = 0x2;
		public static int Hole = 0x4;
		public static int Burns = 0x8;
		public static int BreaksView = 0x10;
		public static int FloorLayer = 0x20;
		public static int LiquidLayer = 0x40;
		public static int WallLayer = 0x80;

		static TileFlags() {
			flags[(int) Tile.Chasm] = Hole | FloorLayer;
			flags[(int) Tile.Dirt] = Passable | LiquidLayer;
			flags[(int) Tile.Grass] = Passable | Burns | LiquidLayer;
			flags[(int) Tile.FloorA] = Passable | FloorLayer;
			flags[(int) Tile.WallA] = Solid | WallLayer | BreaksView;
			flags[(int) Tile.WallB] = Solid | WallLayer | BreaksView;
			flags[(int) Tile.Crack] = Solid | WallLayer | BreaksView;
			flags[(int) Tile.Water] = Passable | LiquidLayer;
			flags[(int) Tile.Venom] = LiquidLayer;
			flags[(int) Tile.WallSide] = FloorLayer;
			flags[(int) Tile.FloorB] = Passable | Burns | FloorLayer;
			flags[(int) Tile.FloorC] = Passable | FloorLayer;
			flags[(int) Tile.FloorD] = Passable | FloorLayer;
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