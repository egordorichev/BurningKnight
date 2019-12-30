using Lens.entity;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class OrbitalComponent : Component {
		public float Radius = 20;
		public bool Lerp;
		public Entity Orbiting;
	}
}