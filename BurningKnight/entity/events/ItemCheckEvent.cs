using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ItemCheckEvent : Event {
		public Item Item;
		public bool Animate;
		public bool Blocked;
	}
}