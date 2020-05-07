using BurningKnight.entity.component;
using Lens.graphics;
using Lens.graphics.animation;

namespace BurningKnight.entity.projectile {
	public class BasicProjectileGraphicsComponent : SliceComponent {
		public BasicProjectileGraphicsComponent(string image, string slice) : base(image, slice) {
			
		}

		public virtual void RenderLight() {

		}
	}
}