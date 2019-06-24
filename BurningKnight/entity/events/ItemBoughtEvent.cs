using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ItemBoughtEvent : Event {
		public Item Item;
		public Entity Who;
		public ShopStand Stand;
	}
}