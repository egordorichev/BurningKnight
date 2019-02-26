using Box2DX.Dynamics;
using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class BodyComponent : Component {
		public Body Body;
		
		public virtual bool ShouldCollide(Entity entity) {
			return true;
		}
		
		public virtual void OnCollision(Entity entity) {
			
		}

		public virtual void OnCollisionEnd(Entity entity) {
			
		}
	}
}