using System.Collections.Generic;
using BurningKnight.level.biome;
using Lens.util.math;

namespace BurningKnight.entity.creature.mob.boss {
	public class BossPatternSetRegistry<T> where T : Boss {
		private List<BossPatternSet<T>> defined = new List<BossPatternSet<T>>();
		private List<float> chances = new List<float>();
		
		public void Register(BossPatternSet<T> set, float chance = 1f, params string[] biomes) {
			defined.Add(set);
			chances.Add(chance);

			set.Biomes = biomes;
		}

		public BossPatternSet<T> Generate(string biome) {
			var types = new List<BossPatternSet<T>>();
			var ch = new List<float>();

			for (var i = 0; i < defined.Count; i++) {
				if (Biome.IsPresent(biome, defined[i].Biomes)) {
					types.Add(defined[i]);
					ch.Add(chances[i]);
				}
			}

			return types[Rnd.Chances(ch)];
		}
	}
}