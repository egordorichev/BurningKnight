using System.Collections.Generic;
using BurningKnight.level.biome;
using Lens.util.math;

namespace BurningKnight.level.paintings {
	public static class PaintingRegistry {
		private static List<Info> paintings = new List<Info>();

		static PaintingRegistry() {
			Add("rexcellent", "egordorichev", 0.1f);
			Add("grannylisa", "DSF100");
			Add("maanex", "DSF100", 0.1f);
			Add("bk", "Mate");
			Add("failpositive", "Jusiv_", 0.1f);
			Add("old_man", "???");
			Add("arthouse", "xD");
			Add("black", "!!!");
			Add("milt", "egordorichev");
			Add("skyscraper", "egordorichev");
			Add("egor", "egordorichev", 0.25f);
			Add("null", "SEGFAULT", 0.5f);
			Add("lamp", "Brastin");
			Add("badosz", "DSF100", 1f, null, true);
			Add("tv", "ANIVIRE");
			Add("company", "ANIVIRE");
			Add("trasevol", "TRASEVOL_DOG");
			Add("scream", "???");
			Add("stars", "???");
			Add("fog", "???");
			Add("cat", "Brastin", 0.1f);
			Add("dungeon", "Johan Peitz", 0.2f);
			Add("goose", "Jaxetly");
			Add("chess", "Nikithn");
			Add("guitar", "Nikithn", 0.5f);
			Add("peach", "unije");
			Add("agency", "smashy", 0.01f);
			Add("ducktective", "smashy", 0.1f);
			Add("beet_boys", "Gaziter");
			Add("moonshine", "Gaziter");
			Add("void", "Gaziter");
			Add("esty", "???", 0.1f);
			
			Add("nat", "MateCziner");
			Add("totemori", "MateCziner");
			Add("horatio", "MateCziner");
			Add("coce", "MateCziner");
			Add("no_idea", "MateCziner");
			Add("tinkerer", "MateCziner");
			Add("totemori_redux", "MateCziner");
			Add("raj", "MateCziner");
			Add("plank", "MateCziner");
			Add("qrilin", "MateCziner");
			Add("in_loving_memory_of_ali", "MateCziner");
			Add("happy_accident", "MateCziner");
			Add("observing_cheese", "MateCziner");
			Add("kobra_throne", "MateCziner");
			Add("chicken_enemy_unknown", "MateCziner");
			Add("mori", "MateCziner");
			Add("balbo", "MateCziner");
			Add("know_stuff", "MateCziner");
			Add("olpi", "MateCziner");
			Add("one_knight_stand", "MateCziner");
			Add("ne_furdje_le", "MateCziner");
			Add("sushi_sushi", "MateCziner");
			Add("hoop_gang", "MateCziner");
			Add("riveting_view", "MateCziner");
			Add("gang", "MateCziner");
			Add("zweihandler", "MateCziner");
			Add("whoops", "MateCziner");
			Add("too_lake", "MateCziner");
			Add("cauliflower", "MateCziner");
			Add("raise_volcano", "MateCziner");
			Add("mahula", "MateCziner");
			Add("step_through", "MateCziner");
			Add("thats_a_moon", "MateCziner");
			Add("totem", "MateCziner");
			Add("too_late", "MateCziner");
			Add("whipped_cream", "MateCziner");
			Add("tofulama", "Didu");
			Add("peasants", "Diego Rivera", 0.1f);
			Add("code", "egordorichev");
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

			float value = Rnd.Float(sum);
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