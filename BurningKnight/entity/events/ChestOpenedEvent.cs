using BurningKnight.entity.chest;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ChestOpenedEvent : Event {
		public Chest Chest;
		public Entity Who;
	}
}