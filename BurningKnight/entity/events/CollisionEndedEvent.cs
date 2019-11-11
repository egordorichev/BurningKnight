using BurningKnight.entity.component;
using Lens.entity;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.events {
	public class CollisionEndedEvent : Event {
		public Entity Entity;
		public BodyComponent Body;
	}
}