using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Graphics {
	public static class Renderer {
		public static SpriteBatch Batch;
		
		public static void Init() {
			Batch = new SpriteBatch(Engine.GraphicsDevice);
		}

		public static void Destroy() {
			Batch.Dispose();
		}

		public static void Clear(Color color) {
			Engine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, color, 1, 0);
		}
	}
}