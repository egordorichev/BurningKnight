using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class OwnerComponent : Component {
		public Entity Owner;

		public OwnerComponent(Entity owner) {
			Owner = owner;
		}
	}
}