using System.Collections.Generic;

namespace BurningKnight.level.biome {
	public static class BiomeRegistry {
		public static Dictionary<string, BiomeInfo> Defined = new Dictionary<string, BiomeInfo>();

		static BiomeRegistry() {
			BiomeInfo[] infos = {
				BiomeInfo.New<CastleBiome>(Biome.Castle),
				BiomeInfo.New<HubBiome>(Biome.Hub),
				BiomeInfo.New<LibraryBiome>(Biome.Library),
				BiomeInfo.New<DesertBiome>(Biome.Desert),
				BiomeInfo.New<IceBiome>(Biome.Ice),
				BiomeInfo.New<ForestBiome>(Biome.Forest)
			};
			
			foreach (var info in infos) {
				Add(info);
			}
		}

		public static BiomeInfo Get(string id) {
			return Defined[id];
		}

		public static BiomeInfo ForDepth(int depth) {
			if (depth == -1) {
				return Defined[Biome.Hub];
			}

			if (depth == 3 || depth == 4) {
				return Defined[Biome.Desert];
			}
			
			return Defined[Biome.Castle];
		}
		
		public static void Add(BiomeInfo info) {
			Defined[info.Id] = info;
		}

		public static void Remove(string id) {
			Defined.Remove(id);
		}
	}
}