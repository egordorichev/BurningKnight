using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.mob.castle;
using BurningKnight.entity.creature.mob.desert;
using BurningKnight.entity.creature.mob.ice;
using BurningKnight.entity.creature.mob.jungle;
using BurningKnight.entity.creature.mob.library;
using BurningKnight.level.biome;
using BurningKnight.state;
using Lens.util.math;
using Buffer = BurningKnight.entity.creature.mob.library.Buffer;

namespace BurningKnight.entity.creature.mob {
	public static class MobRegistry {
		public static List<MobInfo> All = new List<MobInfo>();
		public static List<MobInfo> Current = new List<MobInfo>();
		
		static MobRegistry() {
			MobInfo[] infos = {
				// XD
				MobInfo.New<Dummy>(new SpawnChance(0.1f, Biome.Castle)).SetSpawnChance(0.5f),
				// Castle
				MobInfo.New<Ghost>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<Rat>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<WallCrawler>(new SpawnChance(0.5f + 3f, Biome.Castle)).RequiresNearWall(),
				MobInfo.New<Bandit>(new SpawnChance(1f, Biome.Desert, Biome.Jungle)),
				MobInfo.New<SimpleSlime>(new SpawnChance(1f, Biome.Castle)),
				MobInfo.New<MotherSlime>(new SpawnChance(0.5f, Biome.Castle)).MarkSingle(),
				MobInfo.New<SimpleSlime>(new SpawnChance(0.1f, Biome.Castle)),

				MobInfo.New<Bandit>(new SpawnChance(1f, Biome.Castle)).DisableFirstSpawn(),
				MobInfo.New<Gunner>(new SpawnChance(0.1f, Biome.Castle)).DisableFirstSpawn().SetWeight(3f),
				MobInfo.New<BulletSlime>(new SpawnChance(2f, Biome.Castle)).DisableFirstSpawn(),
				MobInfo.New<BlueBulletSlime>(new SpawnChance(0.1f, Biome.Castle)).DisableFirstSpawn().SetWeight(2f),
				MobInfo.New<Clown>(new SpawnChance(2f, Biome.Castle)).DisableFirstSpawn(),
				
				// Desert
				MobInfo.New<Gunner>(new SpawnChance(2f, Biome.Desert)).SetWeight(1.5f),
				MobInfo.New<DesertSlime>(new SpawnChance(1f, Biome.Desert)),
				MobInfo.New<Maggot>(new SpawnChance(1f, Biome.Desert)).RequiresNearWall(),
				MobInfo.New<Mummy>(new SpawnChance(1f, Biome.Desert)),
				MobInfo.New<Worm>(new SpawnChance(1f, Biome.Desert)),
				MobInfo.New<Spelunker>(new SpawnChance(1f, Biome.Desert), new LoopChance(1f, Biome.Castle)),
				MobInfo.New<Fly>(new SpawnChance(1f, Biome.Desert), new LoopChance(1f, Biome.Castle)),
				
				MobInfo.New<DesertBulletSlime>(new SpawnChance(1f, Biome.Desert)).DisableFirstSpawn().SetWeight(2f).MarkSingle(),
				MobInfo.New<MegaSlime>(new SpawnChance(1f, Biome.Desert)).DisableFirstSpawn().SetWeight(2f).MarkSingle(),
				// MobInfo.New<Cactus>(new SpawnChance(0f, Biome.Desert)).DisableFirstSpawn(),
				
				// Jungle
				MobInfo.New<Sniper>(new SpawnChance(1f, Biome.Jungle), new LoopChance(1f, Biome.Desert)),
				MobInfo.New<BeeHive>(new SpawnChance(100.5f, Biome.Jungle)).MarkSingle().SetWeight(3f).HatesWall(),
				MobInfo.New<Bee>(new SpawnChance(0.3f, Biome.Jungle)),
				MobInfo.New<Explobee>(new SpawnChance(0.15f, Biome.Jungle)),
				MobInfo.New<Wombat>(new SpawnChance(0.7f, Biome.Jungle), new LoopChance(1f, Biome.Desert)).SetWeight(2f).MarkSingle(),
				MobInfo.New<Flower>(new SpawnChance(1f, Biome.Jungle)).SetWeight(2f).HatesWall(),
				
				MobInfo.New<BuffedFlower>(new SpawnChance(1f, Biome.Jungle)).SetWeight(2f).DisableFirstSpawn().HatesWall(),
				
				// Ice
				MobInfo.New<CupGuy>(new SpawnChance(1f, Biome.Ice), new LoopChance(1f, Biome.Jungle)),
				MobInfo.New<Snowman>(new SpawnChance(1f, Biome.Ice)).SetSpawnChance(0.5f).HatesWall(),
				MobInfo.New<Snowball>(new SpawnChance(1f, Biome.Ice)).SetWeight(0.5f),
				MobInfo.New<IceCrawler>(new SpawnChance(1f, Biome.Ice)).RequiresNearWall(),
				MobInfo.New<Snowflake>(new SpawnChance(0.5f, Biome.Ice)).SetSpawnChance(0.5f),
				MobInfo.New<BigSnowball>(new SpawnChance(1f, Biome.Ice)),
				
				MobInfo.New<Sponge>(new SpawnChance(1f, Biome.Ice)).MarkSingle().HatesWall().DisableFirstSpawn(),
				MobInfo.New<Dino>(new SpawnChance(1f, Biome.Ice)).SetWeight(3f).DisableFirstSpawn(),
				
				// Library
				MobInfo.New<Book>(new SpawnChance(1f, Biome.Library)).HatesWall().SetWeight(2.5f),
				MobInfo.New<TeleportingMage>(new SpawnChance(1f, Biome.Library)).HatesWall(),
				MobInfo.New<Skeleton>(new SpawnChance(1f, Biome.Library)).SetSpawnChance(0.5f),
				MobInfo.New<Buffer>(new SpawnChance(1f, Biome.Library), new LoopChance(1f, Biome.Jungle), new LoopChance(1f, Biome.Ice)).MarkSingle(),
				MobInfo.New<DesertSlime>(new SpawnChance(1f, Biome.Library)),
				MobInfo.New<DesertBulletSlime>(new SpawnChance(1f, Biome.Library)),
				
				// Tech for dm fight
				MobInfo.New<DesertBulletSlime>(new SpawnChance(1f, Biome.Tech)),
				MobInfo.New<Book>(new SpawnChance(1f, Biome.Tech)).HatesWall(),
				MobInfo.New<IceCrawler>(new SpawnChance(1f, Biome.Tech)).RequiresNearWall(),
				MobInfo.New<Dino>(new SpawnChance(1f, Biome.Tech)),
				MobInfo.New<Gunner>(new SpawnChance(2f, Biome.Castle)),
			};
			
			All.AddRange(infos);
		}

		public static MobInfo FindFor(Type type) {
			foreach (var info in All) {
				if (info.Type == type) {
					return info;
				}
			}

			return null;
		}

		public static Mob Generate() {
			var chances = new float[Current.Count];

			for (var i = 0; i < Current.Count; i++) {
				if (Current[i].Type == typeof(BeeHive)) {
					continue;
				}
				
				chances[i] = Current[i].GetChanceFor(Run.Level.Biome.Id).Chance;
			}

			var types = new List<MobInfo>();
			var spawnChances = new List<float>();

			for (int i = 0; i < Rnd.Int(2, 6); i++) {
				var type = Current[Rnd.Chances(chances)];
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
					spawnChances.Add(type.Chance);
				}
			}

			if (types.Count == 0) {
				return null;
			}

			return (Mob) Activator.CreateInstance(types[Rnd.Chances(spawnChances)].Type);
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