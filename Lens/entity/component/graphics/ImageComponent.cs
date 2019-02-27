using Lens.assets;
using Lens.graphics;

namespace Lens.entity.component.graphics {
	public class ImageComponent : GraphicsComponent {
		public TextureRegion Sprite;
		
		public ImageComponent(string image) {
			Sprite = Textures.Get(image);

			Entity.Width = Sprite.Source.Width;
			Entity.Height = Sprite.Source.Height;
		}
		
		public override void Render() {
			Graphics.Render(Sprite, Entity.Position);
		}
	}
}