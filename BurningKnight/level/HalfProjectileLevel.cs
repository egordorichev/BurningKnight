using BurningKnight.entity.projectile;
using BurningKnight.physics;
using Lens.entity;

namespace BurningKnight.level {
	public class HalfProjectileLevel : Entity, CollisionFilterEntity {
		public Level Level;

		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			
			AddComponent(new HalfProjectileBodyComponent {
				Level = Level
			});
		}

		public bool ShouldCollide(Entity entity) {
			return entity is Projectile;
		}
	}
}