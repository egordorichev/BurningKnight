using System.Collections.Generic;

namespace BurningKnight.level.biome {
	public static class BiomeRegistry {
		public static Dictionary<string, BiomeInfo> Defined = new Dictionary<string, BiomeInfo>();

		static BiomeRegistry() {
			BiomeInfo[] infos = {
				BiomeInfo.New<HubBiome>(Biome.Hub),

				BiomeInfo.New<CastleBiome>(Biome.Castle),
				BiomeInfo.New<LibraryBiome>(Biome.Library),
				BiomeInfo.New<DesertBiome>(Biome.Desert),
				BiomeInfo.New<IceBiome>(Biome.Ice),
				BiomeInfo.New<ForestBiome>(Biome.Forest),
				BiomeInfo.New<TechBiome>(Biome.Tech)
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

			/*if (depth == 5 || depth == 6) {
				return Defined[Biome.Forest];
			}

			if (depth == 7 || depth == 8) {
				return Defined[Biome.Ice];
			}

			if (depth == 9 || depth == 10) {
				return Defined[Biome.Library];
			}

			// Off the root for now
			if (depth == 11 || depth == 12) {
				return Defined[Biome.Tech];
			}*/
			
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