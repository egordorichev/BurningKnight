using System.Collections.Generic;
using BurningKnight.entity.level.biome;

namespace BurningKnight.entity.level {
	public static class BiomeRegistry {
		public static Dictionary<string, BiomeInfo> Defined = new Dictionary<string, BiomeInfo>();

		static BiomeRegistry() {
			BiomeInfo[]  infos = {
				BiomeInfo.New<CastleBiome>(Biome.Castle)
			};
			
			foreach (var info in infos) {
				Add(info);
			}
		}

		public static BiomeInfo ForDepth(int depth) {
			return Defined[Biome.Castle]; // TODO: other biomes
		}
		
		public static void Add(BiomeInfo info) {
			Defined[info.Id] = info;
		}

		public static void Remove(string id) {
			Defined.Remove(id);
		}
	}
}