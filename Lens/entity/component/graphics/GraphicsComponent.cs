using Microsoft.Xna.Framework;

namespace Lens.entity.component.graphics {
	public class GraphicsComponent : Component {
		public bool Flipped;
		public bool FlippedVerticaly;
		public bool CustomFlip;
		public Vector2 Offset;
		
		public virtual void Render() {
			
		}
	}
}