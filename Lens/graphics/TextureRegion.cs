using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.graphics {
	public class TextureRegion {
		public Texture2D Texture;
		public Rectangle Source;
		public Vector2 Center;
		
		public TextureRegion() {
			
		}

		public TextureRegion(Texture2D texture) {
			Texture = texture;
			Source = texture.Bounds;
			Center = new Vector2(Source.Width / 2f, Source.Height / 2f);
		}

		public TextureRegion(Texture2D texture, Rectangle source) {
			Texture = texture;
			Source = source;
			Center = new Vector2(Source.Width / 2f, Source.Height / 2f);
		}

		public void Set(TextureRegion region) {
			if (region == null) {
				return;
			}
			
			Texture = region.Texture;
			Source = new Rectangle(region.Source.X, region.Source.Y, region.Source.Width, region.Source.Height);
			Center = new Vector2(Source.Width / 2f, Source.Height / 2f);
		}
	}
}