using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.creature.mob.desert;
using BurningKnight.level.biome;
using BurningKnight.state;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.mob {
	public static class MobRegistry {
		public static List<MobInfo> All = new List<MobInfo>();
		public static List<MobInfo> Current = new List<MobInfo>();
		
		static MobRegistry() {
			MobInfo[] infos = {
				// XD
				MobInfo.New<Dummy>(new SpawnChance(0.1f, Biome.Castle)),
				// Castle
				MobInfo.New<Ghost>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<WallCrawler>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<Bandit>(new SpawnChance(1f, Biome.Castle, Biome.Desert)),
				MobInfo.New<SimpleSlime>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<MotherSlime>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<Gunner>(new SpawnChance(0.5f, Biome.Castle)).DisableFirstSpawn(),
				MobInfo.New<BulletSlime>(new SpawnChance(1f, Biome.Castle)).DisableFirstSpawn(),
				MobInfo.New<Clown>(new SpawnChance(1f, Biome.Castle)).DisableFirstSpawn(),
				// Desert
				MobInfo.New<DesertSlime>(new SpawnChance(1f, Biome.Desert)),
				MobInfo.New<Maggot>(new SpawnChance(1f, Biome.Desert)),
				MobInfo.New<Mummy>(new SpawnChance(1f, Biome.Desert)),
				MobInfo.New<Worm>(new SpawnChance(1f, Biome.Desert)),
				MobInfo.New<Spelunker>(new SpawnChance(1f, Biome.Desert)),
				MobInfo.New<Fly>(new SpawnChance(1f, Biome.Desert)),
			};
			
			All.AddRange(infos);
		}

		public static Mob Generate() {
			var chances = new float[Current.Count];

			for (int i = 0; i < Current.Count; i++) {
				chances[i] = Current[i].GetChanceFor(Run.Level.Biome.Id).Chance;
			}

			var types = new List<Type>();
			var spawnChances = new List<float>();

			for (int i = 0; i < Random.Int(2, 6); i++) {
				var type = Current[Random.Chances(chances)].Type;
				var found = false;
				
				foreach (var t in types) {
					if (t == type) {
						found = true;
						break;
					}
				}

				if (found) {
					i--;
				} else {
					types.Add(type);
					spawnChances.Add(((Mob) Activator.CreateInstance(type)).GetSpawnChance());
				}
			}

			if (types.Count == 0) {
				return null;
			}

			return (Mob) Activator.CreateInstance(types[Random.Chances(spawnChances)]);
		}

		public static void SetupForBiome(string biome) {
			Current.Clear();

			foreach (var info in All) {
				if (info.SpawnsIn(biome) && (info.SpawnsOnFirst || Run.Depth % 2 == 0)) {
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