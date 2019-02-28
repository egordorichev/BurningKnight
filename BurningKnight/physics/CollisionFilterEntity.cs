using Lens.entity;

namespace BurningKnight.physics {
	public interface CollisionFilterEntity {
		bool ShouldCollide(Entity entity);
	}
}