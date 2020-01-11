using Lens.entity;

namespace BurningKnight.entity.events {
	public class RevivedEvent : Event {
		public Entity Who;
		public Entity WhoDamaged;
	}
}