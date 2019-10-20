using System.Collections.Generic;

namespace BurningKnight.level.biome {
	public static class BiomeRegistry {
		public static Dictionary<string, BiomeInfo> Defined = new Dictionary<string, BiomeInfo>();

		static BiomeRegistry() {
			BiomeInfo[] infos = {
				// BiomeInfo.New<HubBiome>(Biome.Hub),

				BiomeInfo.New<CastleBiome>(Biome.Castle).Add(0, 1f).Add(1, 1f),
				BiomeInfo.New<DesertBiome>(Biome.Desert).Add(0, 1f).Add(1, 1f),
				BiomeInfo.New<ForestBiome>(Biome.Forest).Add(0, 1f).Add(1, 1f),
				BiomeInfo.New<LibraryBiome>(Biome.Library).Add(0, 1f).Add(1, 1f),
				BiomeInfo.New<TechBiome>(Biome.Tech).Add(0, 1f).Add(1, 1f),
				BiomeInfo.New<IceBiome>(Biome.Ice).Add(0, 1f).Add(1, 1f)
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
			
			if (depth == 2) {
				return Defined[Biome.Desert];
			}
			
			if (depth == 3) {
				return Defined[Biome.Library];
			}

			if (depth == 4) {
				return Defined[Biome.Forest];
			}

			if (depth == 5) {
				return Defined[Biome.Ice];
			}

			if (depth == 6) {
				return Defined[Biome.Tech];
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