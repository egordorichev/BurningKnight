using System.Collections.Generic;
using BurningKnight.level.biome;
using Lens.util.math;

namespace BurningKnight.level.paintings {
	public static class PaintingRegistry {
		private static List<Info> paintings = new List<Info>();

		static PaintingRegistry() {
			Add("rexcellent", "egordorichev");
			Add("grannylisa", "DSF100");
			Add("maanex", "DSF100");
			Add("bk", "Mate");
			Add("failpositive", "Jusiv_");
			Add("old_man", "???");
			Add("arthouse", "xD");
			Add("black", "!!!");
			Add("milt", "egordorichev");
			Add("skyscraper", "egordorichev");
			Add("egor", "egordorichev", 0.25f);
			Add("null", "SEGFAULT", 0.5f);
			Add("lamp", "Brastin");
			Add("banana", "Minions");
			Add("badosz", "DSF100", 1f, null, true);
			Add("tv", "ANIVIRE");
			Add("company", "ANIVIRE");
			Add("pico", "zep");
			Add("liko", "Rami");
			Add("trasevol", "TRASEVOL_DOG");
			Add("scream", "???");
			Add("stars", "???");
			Add("fog", "???");
			Add("nufflee", "Nufflee");
			Add("car", "???");
			Add("moika", "???");
			Add("sunset", "???");
		}
		
		public static void Add(string id, string author, float chance = 1f, string[] biomes = null, bool animated = false) {
			paintings.Add(new Info {
				Id = id,
				Author = author,
				Chance = chance,
				Biomes = biomes,
				Animated = animated
			});
		}

		public static Painting Generate(Biome biome) {
			var length = paintings.Count;
			float sum = 0;

			foreach (var info in paintings) {
				if (biome.IsPresent(info.Biomes)) {
					sum += info.Chance;
				}
			}

			float value = Random.Float(sum);
			sum = 0;

			for (int i = 0; i < length; i++) {
				var info = paintings[i];

				if (!biome.IsPresent(info.Biomes)) {
					continue;
				}

				sum += info.Chance;

				if (value < sum) {
					if (info.Animated) {
						return new AnimatedPainting {
							Id = info.Id,
							Author = info.Author
						};
					}
					
					return new Painting {
						Id = info.Id,
						Author = info.Author
					};
				}
			}

			return null;
		}

		private class Info {
			public string Id;
			public string Author;
			public float Chance;
			public string[] Biomes;
			public bool Animated;
		}
	}
}