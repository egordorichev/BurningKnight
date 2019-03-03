using System.Collections.Generic;
using BurningKnight.entity.creature.mob.castle;

namespace BurningKnight.entity.creature.mob {
	public static class MobRegistry {
		private static MobInfo[] infos = {
			MobInfo.New<Knight>(new SpawnChance(1f, "castle", "library"), new SpawnChance(0.1f, "secret_area"))
		};
		
		public static List<MobInfo> All = new List<MobInfo>();

		static MobRegistry() {
			All.AddRange(infos);
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