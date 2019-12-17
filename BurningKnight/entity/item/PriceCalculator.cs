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
		
		public static int Calculate(Item item) {
			return BasePrice(item.Type) * (Curse.IsEnabled(Curse.OfGreed) ? 2 : 1);
		}
	}
}