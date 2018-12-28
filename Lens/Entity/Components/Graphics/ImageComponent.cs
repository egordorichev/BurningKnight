using System.IO;
using Lens.Asset;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Entity.Components.Graphics {
	public class ImageComponent : GraphicsComponent {
		private Texture2D texture;
		
		public ImageComponent(string image) {
			var fileStream = new FileStream(image, FileMode.Open);
			texture = Texture2D.FromStream(Engine.Graphics.GraphicsDevice, fileStream);
			fileStream.Dispose();		
		}
		
		public override void Render() {
			Renderer.Batch.Begin();
			Renderer.Batch.Draw(texture, Entity.Position, Color.White);
			Renderer.Batch.End();
		}

		public override void Destroy() {
			base.Destroy();
			texture.Dispose();
		}
	}
}