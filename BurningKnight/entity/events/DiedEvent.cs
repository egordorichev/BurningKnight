using Lens.entity;

namespace BurningKnight.entity.events {
	public class DiedEvent : Event {
		public Entity From;
		public Entity Who;
		public bool BlockClear;
		public DamageType DamageType;
	}
}