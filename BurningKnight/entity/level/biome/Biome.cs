using Lens;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.biome {
	public class Biome {
		public const string Castle = "castle";
		
		public readonly string Music;
		public readonly string Id;
		public readonly string Tileset;

		public Biome(string music, string id, string tileset, Color bg) {
			Music = music;
			Id = id;
			Tileset = tileset;
			Engine.Instance.StateRenderer.Bg = bg;
		}
	}
}