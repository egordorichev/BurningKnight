using System;

namespace BurningKnight.entity.creature.mob {
	public class MobInfo {
		public Type Type;
		public SpawnChance[] Spawns;
		public bool SpawnsOnFirst = true;
		public bool NearWall;
		public bool Single;
		public bool AwayFromWall;
		public float Weight = 1;
		public float Chance = 1;

		public static MobInfo New<T>(params SpawnChance[] spawns) where T : Mob {
			return new MobInfo {
				Type = typeof(T),
				Spawns = spawns
			};
		}
		
		public MobInfo MarkSingle() {
      Single = true;
      return this;
		}
		
		public MobInfo SetWeight(float weight) {
	    Weight = weight;
	    return this;
		}
		
		public MobInfo SetSpawnChance(float chance) {
			Chance = chance;
			return this;
		}

		public MobInfo RequiresNearWall() {
			NearWall = true;
			AwayFromWall = false;
			return this;
		}

		public MobInfo HatesWall() {
			NearWall = false;
			AwayFromWall = true;
			return this;
		}

		public MobInfo DisableFirstSpawn() {
			SpawnsOnFirst = false;
			return this;
		}

		public bool SpawnsIn(string biome) {
			return GetChanceFor(biome) != null;
		}

		public SpawnChance GetChanceFor(string biome) {
			foreach (var b in Spawns) {
				foreach (var a in b.Areas) {
					if (a == biome) {
						return b;
					}
				}
			}
			
			return null;
		}
	}
}