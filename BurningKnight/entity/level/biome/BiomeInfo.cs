using System;

namespace BurningKnight.entity.level.biome {
	public class BiomeInfo {
		public Type Type;
		public string Id;

		public BiomeInfo(Type type, string id) {
			Type = type;
			Id = id;
		}

		public static BiomeInfo New<T>(string id) where T : Biome {
			return new BiomeInfo(typeof(T), id);
		}
	}
}