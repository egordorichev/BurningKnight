using BurningKnight.entity.projectile;
using Lens.entity;

namespace BurningKnight.entity.events {
	public class ProjectileCreatedEvent : Event {
		public Projectile Projectile;
		public Entity Owner;
	}
}