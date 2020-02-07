using Lens.entity;

namespace BurningKnight.entity.events {
	public class ExplodedEvent : Event {
		public Entity Who;
		public float Damage;
	}
}