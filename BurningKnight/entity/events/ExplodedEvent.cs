using Lens.entity;

namespace BurningKnight.entity.events {
	public class ExplodedEvent : Event {
		public Entity Who;
		public Entity Origin;
		public float Damage;
	}
}