using System.Collections.Generic;
using BurningKnight.entity.level.biome;

namespace BurningKnight.entity.level {
	public static class BiomeRegistry {
		private static BiomeInfo[] infos = {
			BiomeInfo.New<CastleBiome>(Biome.Castle)
		};
		
		public static Dictionary<string, BiomeInfo> All = new Dictionary<string, BiomeInfo>();

		static BiomeRegistry() {
			foreach (var info in infos) {
				Add(info);
			}
		}
		
		public static void Add(BiomeInfo info) {
			All[info.Id] = info;
		}

		public static void Remove(string id) {
			All.Remove(id);
		}
	}
}