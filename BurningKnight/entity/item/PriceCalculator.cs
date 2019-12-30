using System;

namespace BurningKnight.entity.item {
	public static class PriceCalculator {
		public static int BasePrice(ItemType type) {
			switch (type) {
				case ItemType.Artifact: return 15;
				
				case ItemType.Active:
				case ItemType.Coin: return 10;
				
				case ItemType.Bomb:
				case ItemType.Key:
				case ItemType.Battery:
				case ItemType.Heart: return 3;
				
				case ItemType.Hat: return 1;
				
				default: return 15;
			}
		}

		public static float GetPriceModifier(this ItemQuality quality) {
			switch (quality) {
				case ItemQuality.Wooden: default: return 1;
				case ItemQuality.Iron: return 1.333f;
				case ItemQuality.Golden: return 2f;
			}	
		}

		public static float GetModifier(Item item) {
			return (Scourge.IsEnabled(Scourge.OfGreed) ? 2 : 1) * item.Data.Quality.GetPriceModifier();
		}
		
		public static int Calculate(Item item) {
			return (int) Math.Round(BasePrice(item.Type) * GetModifier(item));
		}
	}
}