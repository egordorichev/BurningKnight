using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.graphics {
	public class TextureRegion {
		public Texture2D Texture;
		public Rectangle Source;
		
		public TextureRegion() {
			
		}

		public TextureRegion(Texture2D texture) {
			Texture = texture;
			Source = texture.Bounds;
		}

		public TextureRegion(Texture2D texture, Rectangle source) {
			Texture = texture;
			Source = source;
		}

		public void Set(TextureRegion region) {
			if (region == null) {
				return;
			}
			
			Texture = region.Texture;
			Source = new Rectangle(region.Source.X, region.Source.Y, region.Source.Width, region.Source.Height);
		}
	}
}