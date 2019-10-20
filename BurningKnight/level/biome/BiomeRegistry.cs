using System.Collections.Generic;
using System.Linq;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.biome {
	public static class BiomeRegistry {
		public static Dictionary<string, BiomeInfo> Defined = new Dictionary<string, BiomeInfo>();

		static BiomeRegistry() {
			BiomeInfo[] infos = {
				// BiomeInfo.New<HubBiome>(Biome.Hub),

				BiomeInfo.New<CastleBiome>(Biome.Castle).Add(0, 1f).Add(1, 1f).Add(-1, 1f).Add(0, 1f),
				BiomeInfo.New<DesertBiome>(Biome.Desert).Add(2, 1f).Add(3, 1f),
				BiomeInfo.New<ForestBiome>(Biome.Forest).Add(4, 1f).Add(5, 1f),
				BiomeInfo.New<LibraryBiome>(Biome.Library).Add(6, 1f).Add(7, 1f),
				BiomeInfo.New<TechBiome>(Biome.Tech).Add(8, 1f).Add(9, 1f),
				BiomeInfo.New<IceBiome>(Biome.Ice).Add(10, 1f).Add(11, 1f)
			};
			
			foreach (var info in infos) {
				Add(info);
			}
		}

		public static BiomeInfo Get(string id) {
			return Defined[id];
		}

		public static BiomeInfo GenerateForDepth(int depth) {
			var list = new List<BiomeInfo>();
			var chances = new List<float>();

			foreach (var b in Defined.Values) {
				if (b.Depths.Contains(depth)) {
					list.Add(b);
					chances.Add(b.Chances[b.Depths.IndexOf(depth)]);
				}
			}

			var index = Random.Chances(chances);

			if (index == -1) {
				Log.Error($"Failed to generate a biome for the depth {depth}");
				return Defined.Values.First();
			}

			return list[index];
		}
		
		public static void Add(BiomeInfo info) {
			Defined[info.Id] = info;
		}

		public static void Remove(string id) {
			Defined.Remove(id);
		}
	}
}