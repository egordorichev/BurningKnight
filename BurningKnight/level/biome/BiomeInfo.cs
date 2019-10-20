using System;
using System.Collections.Generic;

namespace BurningKnight.level.biome {
	public class BiomeInfo {
		public readonly Type Type;
		public readonly string Id;
		public readonly List<int> Depths = new List<int>();
		public readonly List<float> Chances = new List<float>();

		public BiomeInfo(Type type, string id) {
			Type = type;
			Id = id;
		}

		public static BiomeInfo New<T>(string id, params float[] types) where T : Biome {
			return new BiomeInfo(typeof(T), id);
		}

		public BiomeInfo Add(int depth, float chance) {
			Depths.Add(depth);
			Chances.Add(chance);

			return this;
		}
	}
}