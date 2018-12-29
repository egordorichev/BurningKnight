using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens.Graphics {
	public static class Gr {
		public static SpriteBatch Batch;
		public static Color Color = Color.White;
		
		public static void Init() {
			Batch = new SpriteBatch(Engine.GraphicsDevice);
		}

		public static void Destroy() {
			Batch.Dispose();
		}

		public static void Clear(Color color) {
			Engine.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, color, 1, 0);
		}

		public static void Render(TextureRegion region, Vector2 position) {
			Batch.Draw(region.Texture, position, region.Source, Color);
		}

		public static void Render(TextureRegion region, Vector2 position, float a, Vector2 origin) {
			Batch.Draw(region.Texture, position, region.Source, Color, a, origin, Vector2.One, SpriteEffects.None, 0);
		}

		public static void Render(TextureRegion region, Vector2 position, float a, Vector2 origin, Vector2 scale, SpriteEffects flip) {
			Batch.Draw(region.Texture, position, region.Source, Color, a, origin, scale, flip, 0);
		}

		public static void Render(Texture2D texture, Vector2 position) {
			Batch.Draw(texture, position, texture.Bounds, Color);
		}
		
		public static void Render(Texture2D texture, Vector2 position, float a, Vector2 origin) {
			Batch.Draw(texture, position, texture.Bounds, Color, a, origin, Vector2.One, SpriteEffects.None, 0);
		}
		
		public static void Render(Texture2D texture, Vector2 position, float a, Vector2 origin, Vector2 scale, SpriteEffects flip) {
			Batch.Draw(texture, position, texture.Bounds, Color, a, origin, scale, flip, 0);
		}
	}
}