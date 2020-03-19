using System.Collections.Generic;
using System.Linq;
using BurningKnight.state;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.biome {
	public static class BiomeRegistry {
		public static Dictionary<string, BiomeInfo> Defined = new Dictionary<string, BiomeInfo>();
		public static Dictionary<string, BiomeInfo> BossRushDefined = new Dictionary<string, BiomeInfo>();

		static BiomeRegistry() {
			var jungle = BiomeInfo.New<JungleBiome>(Biome.Jungle).Add(5, 1f).Add(6, 1f);
			
			// Just to make old levels load properly, jungle used to be called forest
			Defined["forest"] = jungle;
			
			BiomeInfo[] infos = {
				BiomeInfo.New<HubBiome>(Biome.Hub).Add(0, 1f),

				BiomeInfo.New<CastleBiome>(Biome.Castle).Add(1, 1f).Add(2, 1f).Add(-2, 1f).Add(-1, 1f),
				BiomeInfo.New<DesertBiome>(Biome.Desert).Add(3, 1f).Add(4, 1f),
				jungle,
				BiomeInfo.New<IceBiome>(Biome.Ice).Add(7, 1f).Add(8, 1f),
				BiomeInfo.New<LibraryBiome>(Biome.Library).Add(9, 1f).Add(10, 1f),

				BiomeInfo.New<TechBiome>(Biome.Tech).Add(11, 1f).Add(12, 1f)
			};
			
			foreach (var info in infos) {
				Add(info);
			}
			
			BiomeInfo[] bossRushInfos = {
				BiomeInfo.New<CastleBiome>(Biome.Castle).Add(1, 1f),
				BiomeInfo.New<DesertBiome>(Biome.Desert).Add(2, 1f),
				BiomeInfo.New<JungleBiome>(Biome.Jungle).Add(3, 1f),
				BiomeInfo.New<IceBiome>(Biome.Ice).Add(4, 1f),
				BiomeInfo.New<LibraryBiome>(Biome.Library).Add(5, 1f)
			};
			
			foreach (var info in bossRushInfos) {
				AddBossRush(info);
			}
		}

		public static BiomeInfo Get(string id) {
			return Defined[id];
		}

		public static BiomeInfo GenerateForDepth(int depth) {
			var list = new List<BiomeInfo>();
			var chances = new List<float>();
			var array = Run.Type == RunType.BossRush && depth > 0 ? BossRushDefined : Defined;

			foreach (var b in array.Values) {
				if (b.Depths.Contains(depth)) {
					list.Add(b);
					chances.Add(b.Chances[b.Depths.IndexOf(depth)]);
				}
			}

			var index = Rnd.Chances(chances);

			if (index == -1) {
				Log.Error($"Failed to generate a biome for the depth {depth}");
				return array.Values.First();
			}

			return list[index];
		}
		
		public static void Add(BiomeInfo info) {
			Defined[info.Id] = info;
		}
		
		public static void AddBossRush(BiomeInfo info) {
			BossRushDefined[info.Id] = info;
		}

		public static void Remove(string id) {
			Defined.Remove(id);
		}
	}
}