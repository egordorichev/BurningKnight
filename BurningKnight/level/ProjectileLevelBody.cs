using BurningKnight.entity.projectile;
using BurningKnight.physics;
using Lens.entity;

namespace BurningKnight.level {
	public class ProjectileLevelBody : Entity, CollisionFilterEntity {
		public Level Level;

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new ProjectileBodyComponent {
				Level = Level
			});
		}

		public bool ShouldCollide(Entity entity) {
			return !(entity is Projectile);
		}
	}
}