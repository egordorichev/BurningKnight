using System.Collections.Generic;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.level.biome;

namespace BurningKnight.entity.creature.mob {
	public static class MobRegistry {
		public static List<MobInfo> All = new List<MobInfo>();
		public static List<MobInfo> Current = new List<MobInfo>();
		
		static MobRegistry() {
			MobInfo[] infos = {
				// XD
				MobInfo.New<Dummy>(new SpawnChance(0.1f, Biome.Castle)),
				// Castle enemies, that are ready
				MobInfo.New<Ghost>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<WallCrawler>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<Maggot>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<Bandit>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<SimpleSlime>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<Gunner>(new SpawnChance(0.5f, Biome.Castle)),
				
				
				
				// MobInfo.New<MotherSlime>(new SpawnChance(10000f, Biome.Castle)),
				
				// WIP
				//MobInfo.New<Clown>(new SpawnChance(1f, Biome.Castle)),
				//MobInfo.New<Knight>(new SpawnChance(1f, Biome.Castle)),
				//MobInfo.New<King>(new SpawnChance(1f, Biome.Castle))
			};
			
			All.AddRange(infos);
		}
		
		public static void SetupForBiome(string biome) {
			Current.Clear();

			foreach (var info in All) {
				if (info.SpawnsIn(biome)) {
					Current.Add(info);
				}
			}
		}

		public static void Remove<T>() where T : Mob {
			var type = typeof(T);
			MobInfo i = null; 
			
			foreach (var info in All) {
				if (info.Type == type) {
					i = info;
					break;
				}
			}

			if (i != null) {
				All.Remove(i);
			}
		}
	}
}