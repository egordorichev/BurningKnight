using System.Collections.Generic;

namespace BurningKnight.entity.level {
	public class Tilesets {
		public static Dictionary<string, Tileset> Loaded = new Dictionary<string, Tileset>();

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