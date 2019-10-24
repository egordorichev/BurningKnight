using System;

namespace BurningKnight.entity.creature.mob {
	public class MobInfo {
		public Type Type;
		public SpawnChance[] Spawns;
		public bool SpawnsOnFirst = true;

		public static MobInfo New<T>(params SpawnChance[] spawns) where T : Mob {
			return new MobInfo {
				Type = typeof(T),
				Spawns = spawns
			};
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