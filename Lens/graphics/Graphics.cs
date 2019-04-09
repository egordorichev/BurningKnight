using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Lens.graphics {
	public static class Graphics {
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

		public static void Render(TextureRegion region, Vector2 position, float a, Vector2 origin, Vector2 scale, SpriteEffects flip = SpriteEffects.None) {
			Batch.Draw(region.Texture, position, region.Source, Color, a, origin, scale, flip, 0);
		}

		public static void Render(Texture2D texture, Vector2 position) {
			Batch.Draw(texture, position, texture.Bounds, Color);
		}
		
		public static void Render(Texture2D texture, Vector2 position, float a, Vector2 origin) {
			Batch.Draw(texture, position, texture.Bounds, Color, a, origin, Vector2.One, SpriteEffects.None, 0);
		}
		
		public static void Render(Texture2D texture, Vector2 position, float a, Vector2 origin, Vector2 scale, SpriteEffects flip = SpriteEffects.None) {
			Batch.Draw(texture, position, texture.Bounds, Color, a, origin, scale, flip, 0);
		}
		
		public static void Print(string str, BitmapFont font, int x, int y) {
			Batch.DrawString(font, str, new Vector2(x + 1, y - 3), Color);
		}

		public static void Print(string str, BitmapFont font, Vector2 position) {
			Batch.DrawString(font, str, new Vector2(position.X + 1, position.Y - 2), Color);
		}

		public static void Print(string str, BitmapFont font, Vector2 position, float angle, Vector2 origin, float scale) {
			Batch.DrawString(font, str, new Vector2(position.X + 1, position.Y - 2), Color, angle, origin, scale, SpriteEffects.None, 0f);
		}

		public static SpriteEffects ParseEffect(bool h, bool v) {
			if (h && v) {
				return SpriteEffects.FlipVertically | SpriteEffects.FlipHorizontally;
			}
			
			if (v) {
				return SpriteEffects.FlipVertically;
			}
			
			return h ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		}
	}
}