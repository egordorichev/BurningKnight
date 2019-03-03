namespace BurningKnight.entity.level.biome {
	public class Biome {
		public const string Castle = "castle";
		
		public readonly string Music;
		public readonly string Id;
		public readonly string Tileset;

		public Biome(string music, string id, string tileset) {
			Music = music;
			Id = id;
			Tileset = tileset;
		}
	}
}