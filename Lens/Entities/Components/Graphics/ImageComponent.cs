using System.IO;
using Lens.Asset;
using Lens.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Entities.Components.Graphics {
	public class ImageComponent : GraphicsComponent {
		private Texture2D texture;
		
		public ImageComponent(string image) {
			var fileStream = new FileStream(image, FileMode.Open);
			texture = Texture2D.FromStream(Engine.GraphicsDev, fileStream);
			fileStream.Dispose();		
		}
		
		public override void Render() {
			/*
			 * Texture2D texture,
				Vector2 position,
				Rectangle? sourceRectangle,
				Color color,
				float rotation,
				Vector2 origin,
				float scale,
				SpriteEffects effects,
                float layerDepth
			 */

			Renderer.Batch.Draw(texture, Entity.Position, new Rectangle(0, 0, 8, 8), Color.White, (float) Engine.GameTime.TotalGameTime.TotalSeconds, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
		}

		public override void Destroy() {
			base.Destroy();
			texture.Dispose();
		}
	}
}