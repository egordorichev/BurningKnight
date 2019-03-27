namespace BurningKnight.entity.item {
	public static class PriceCalculator {
		public static int BasePrice(ItemType type) {
			switch (type) {
				case ItemType.Normal: return 15;
				
				case ItemType.Active: return 10;

				case ItemType.Coin: return 99;
				
				case ItemType.Bomb:
				case ItemType.Key:
				case ItemType.Heart: return 3;
				
				default: return 15;
			}
		}
		
		public static int Calculate(Item item) {
			return BasePrice(item.Type) * item.Count;
		}
	}
}