using Lens.Asset;
using Lens.Graphics;

namespace Lens.Entities.Components.Graphics {
	public class ImageComponent : GraphicsComponent {
		private TextureRegion texture;
		
		public ImageComponent(string image) {
			texture = Textures.Get(image);
		}
		
		public override void Render() {
			Gr.Render(texture, Entity.Position);
		}
	}
}