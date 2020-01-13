using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ItemRemovedEvent : Event {
		public Item Item;
		public Entity Owner;
	}
}