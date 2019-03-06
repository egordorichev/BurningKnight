using BurningKnight.entity.item;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ConsumableRemovedEvent : Event {
		public int Amount;
		public int TotalNow;
		public ItemType Type;
	}
}