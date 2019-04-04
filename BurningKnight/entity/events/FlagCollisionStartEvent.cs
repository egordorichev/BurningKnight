using Lens.entity;

namespace BurningKnight.entity.events {
	public class FlagCollisionStartEvent : Event {
		public int Flag;
		public Entity Who;
	}
}