using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Graphics {
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
	}
}