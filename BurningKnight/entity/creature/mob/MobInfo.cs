using System;

namespace BurningKnight.entity.creature.mob {
	public class MobInfo {
		public Type Type;
		public SpawnChance[] Spawns;

		public static MobInfo New<T>(params SpawnChance[] spawns) where T : Mob {
			return new MobInfo {
				Type = typeof(T),
				Spawns = spawns
			};
		}

		public bool SpawnsIn(string biome) {
			foreach (var b in Spawns) {
				foreach (var a in b.Areas) {
					if (a == biome) {
						return true;
					}
				}
			}
			
			return false;
		}
	}
}