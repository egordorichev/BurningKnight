using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Asset {
	public static class Renderer {
		public static SpriteBatch Batch;
		
		public static void Init() {
			Batch = new SpriteBatch(Engine.Graphics.GraphicsDevice);
		}

		public static void Destroy() {
			Batch.Dispose();
		}

		public static void Clear(Color color) {
			Engine.Graphics.GraphicsDevice.Clear(color);
		}
	}
}