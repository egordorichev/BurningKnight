using Lens.entity;

namespace BurningKnight.entity.events {
	public class HealthModifiedEvent : Event {
		public int Amount;
		public Entity From;
		public bool Default = true;
	}
}