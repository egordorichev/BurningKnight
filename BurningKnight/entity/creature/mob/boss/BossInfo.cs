using System;

namespace BurningKnight.entity.creature.mob.boss {
	public class BossInfo {
		public Type Type;
		public SpawnChance[] Spawns;

		public static BossInfo New<T>(params SpawnChance[] spawns) where T : Mob {
			return new BossInfo {
				Type = typeof(T),
				Spawns = spawns
			};
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