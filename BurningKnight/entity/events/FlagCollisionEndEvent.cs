using Lens.entity;

namespace BurningKnight.entity.events {
	public class FlagCollisionEndEvent : Event {
		public int Flag;
		public Entity Who;
	}
}