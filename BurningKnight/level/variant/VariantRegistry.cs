using System.Collections.Generic;
using BurningKnight.level.biome;
using Lens.util.math;

namespace BurningKnight.level.variant {
	public static class VariantRegistry {
		private static List<VariantInfo> defined = new List<VariantInfo>();

		static VariantRegistry() {
			Define(new RegularLevelVariant(), 1f, Biome.Castle, Biome.Desert, Biome.Jungle, Biome.Ice, Biome.Library);
			Define(new ChasmLevelVariant(), 0.2f, Biome.Castle, Biome.Desert, Biome.Library);
			Define(new SandLevelVariant(), 0.15f, Biome.Castle, Biome.Jungle, Biome.Ice, Biome.Library);
			Define(new FloodedLevelVariant(), 0.15f, Biome.Castle, Biome.Desert, Biome.Jungle, Biome.Ice, Biome.Library);
			Define(new SnowLevelVariant(), 0.05f, Biome.Castle);
			Define(new ForestLevelVariant(), 0.03f, Biome.Castle);
			Define(new WebbedLevelVariant(), 0.02f, Biome.Castle, Biome.Desert, Biome.Jungle, Biome.Ice, Biome.Library);
			Define(new SandLevelVariant(), 0.05f, Biome.Castle, Biome.Desert, Biome.Jungle, Biome.Ice, Biome.Library);
			Define(new GoldLevelVariant(), 0.005f, Biome.Castle);
		}

		public static void Define(LevelVariant variant, float chance, params string[] biomes) {
			defined.Add(new VariantInfo {
				Chance = chance,
				Variant = variant,
				Biomes = biomes
			});
		}

		public static LevelVariant Create(string id) {
			foreach (var variant in defined) {
				if (variant.Variant.Id == id) {
					return variant.Variant;
				}
			}
			
			return new RegularLevelVariant();
		}

		public static LevelVariant Generate(string biome) {
			var variants = new List<LevelVariant>();
			var chances = new List<float>();

			foreach (var variant in defined) {
				foreach (var b in variant.Biomes) {
					if (b == biome) {
						variants.Add(variant.Variant);
						chances.Add(variant.Chance);
						break;
					}
				}
			}
			
			if (variants.Count == 0) {
				return null;
			}

			var index = Rnd.Chances(chances);

			if (index == -1) {
				return new RegularLevelVariant();
			}

			return variants[index];
		}
	}
}