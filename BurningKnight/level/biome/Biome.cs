using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class Biome {
		public const string Castle = "castle";
		public const string Hub = "hub";
		public const string Library = "library";
		public const string Desert = "desert";
		
		public readonly string Music;
		public readonly string Id;
		public readonly string Tileset;
		public readonly Color Bg;

		public Biome(string music, string id, string tileset, Color bg) {
			Music = music;
			Id = id;
			Tileset = tileset;
			Bg = bg;
		}

		public bool IsPresent(string[] biomes) {
			return IsPresent(Id, biomes);
		}

		public static bool IsPresent(string id, string[] biomes) {
			if (biomes == null) {
				return true;
			}

			foreach (var b in biomes) {
				if (b == id) {
					return true;
				}
			}

			return false;
		}
	}
}