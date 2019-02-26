using Lens.assets;
using Lens.graphics;

namespace Lens.entity.component.graphics {
	public class ImageComponent : GraphicsComponent {
		public TextureRegion Sprite;
		
		public ImageComponent(string image) {
			Sprite = Textures.Get(image);
		}
		
		public override void Render() {
			Graphics.Render(Sprite, Entity.Position);
		}
	}
}