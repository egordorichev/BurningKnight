using System.Collections.Generic;
using BurningKnight.entity.level.biome;

namespace BurningKnight.entity.level {
	public static class BiomeRegistry {
		private static BiomeInfo[] infos = {
			BiomeInfo.New<CastleBiome>(Biome.Castle)
		};
		
		public static Dictionary<string, BiomeInfo> Defined = new Dictionary<string, BiomeInfo>();

		static BiomeRegistry() {
			foreach (var info in infos) {
				Add(info);
			}
		}
		
		public static void Add(BiomeInfo info) {
			Defined[info.Id] = info;
		}

		public static void Remove(string id) {
			Defined.Remove(id);
		}
	}
}