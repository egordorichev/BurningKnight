using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ItemTakenEvent : Event {
		public Item Item;
		public Entity Who;
		public ItemStand Stand;
	}
}