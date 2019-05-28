using BurningKnight.entity.door;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class LockClosedEvent : Event {
		public Lock Lock;
		public Entity Who;
	}
}