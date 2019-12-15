using System.Collections.Generic;
using BurningKnight.level.biome;
using Lens.util.math;

namespace BurningKnight.util {
	public static class BiomeTitles {
		private static Dictionary<string, List<string>> defined = new Dictionary<string, List<string>>();

		public static void Add(string biome, params string[] joke) {
			if (!defined.ContainsKey(biome)) {
				defined[biome] = new List<string>();
			}
			
			defined[biome].AddRange(joke);
		}

		public static string Generate(string biome) {
			if (!defined.TryGetValue(biome, out var list)) {
				return "Idk man, kinda 404?";
			}

			return list[Rnd.Int(list.Count)];
		}

		static BiomeTitles() {
			Add(Biome.Castle,
				"This place is old",
				"The knights are gone"
			);
			
			Add(Biome.Desert,
				"Drink water, stay hydrated",
				"Anakin hates sand",
				"It's getting hot"
			);
			
			Add(Biome.Jungle,
				"This is wild",
				"Jungle be like"
			);
		}
	}
}