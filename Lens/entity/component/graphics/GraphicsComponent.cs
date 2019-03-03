using Microsoft.Xna.Framework;

namespace Lens.entity.component.graphics {
	public class GraphicsComponent : Component {
		public bool Flipped;
		public Vector2 Offset;
		
		public virtual void Render() {
			
		}
	}
}