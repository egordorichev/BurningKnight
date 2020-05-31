using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ItemUsedEvent : Event {
		public Item Item;
		public Entity Who;
		public bool Fake;
	}
}