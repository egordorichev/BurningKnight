using Lens.entity;

namespace BurningKnight.entity.events {
	public class InteractedEvent : Event {
		public Entity Who;
		public Entity With;
	}
}