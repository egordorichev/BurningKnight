using BurningKnight.entity.item;
using BurningKnight.entity.item.stand;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ItemPlacedEvent : Event {
		public Item Item;
		public Entity Who;
		public ItemStand Stand;
	}
}