using System;
using System.Collections.Generic;
using BurningKnight.level.biome;
using Lens.util;
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
				Log.Error($"Didn't find title for {biome}");
				return "Idk man, kinda 404?";
			}

			return list[new Random().Next(list.Count)];
		}

		static BiomeTitles() {
			Add(Biome.Hub,
				"Dodge this",
				"Shopkeeper knows something"
			);
			
			Add(Biome.Castle,
				"This place is old",
				"The knights are gone"
			);
			
			Add(Biome.Desert,
				"Drink water, stay hydrated",
				"Anakin hates sand",
				"It's getting hot",
				"Not to be confused with the desert"
			);
			
			Add(Biome.Jungle,
				"This is wild",
				"Jungle be like",
				"This place is growing on me",
				"Like snakes and ladders without ladders"
			);
			
			Add(Biome.Ice,
				"This place is cool",
				"Let it snow",
				"The winter is coming",
				"Ice Age",
				"Snow Inc.",
				"I saw mamonths"
			);
			
			Add(Biome.Library,
				"Try to pillage quieter, please!",
				"Read a book"
			);
		}
	}
}