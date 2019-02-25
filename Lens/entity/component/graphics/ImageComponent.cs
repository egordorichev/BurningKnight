using Lens.assets;
using Lens.graphics;

namespace Lens.entity.component.graphics {
	public class ImageComponent : GraphicsComponent {
		private TextureRegion texture;
		
		public ImageComponent(string image) {
			texture = Textures.Get(image);
		}
		
		public override void Render() {
			Graphics.Render(texture, Entity.Position);
		}
	}
}