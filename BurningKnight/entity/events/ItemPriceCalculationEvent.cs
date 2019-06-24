using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ItemPriceCalculationEvent : Event {
		public Item Item;
		public ShopStand Stand;
		public float Percent;
	}
}