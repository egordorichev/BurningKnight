using System.Collections.Generic;
using BurningKnight.entity.level.biome;

namespace BurningKnight.entity.level.tile {
	public class Tilesets {
		public static Dictionary<string, Tileset> Loaded = new Dictionary<string, Tileset>();
		public static BiomeAssets Biome;

		public static void Load() {
			if (Biome == null) {
				Biome = new BiomeAssets();
			}
		}
		
		public static Tileset Get(string id) {
			Tileset tileset;
			
			if (Loaded.TryGetValue(id, out tileset)) {
				return tileset;
			}
			
			tileset = new Tileset();
			tileset.Load(id);
			Loaded[id] = tileset;
			
			return tileset;
		}
	}
}