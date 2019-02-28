using Lens.entity;

namespace BurningKnight.physics {
	public interface CollisionListenerEntity {
		void OnCollision(Entity entity);
		void OnCollisionEnd(Entity entity);
	}
}