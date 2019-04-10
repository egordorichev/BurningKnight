using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Ctrings {
	public class BitmapCFont : CFont {
		public BitmapFont Font;
		public SpriteBatch Batch;
		
		public BitmapCFont(BitmapFont font, SpriteBatch batch) {
			Font = font;
			Batch = batch;
		}
		
		public override Vector2 MeasureString(string text) {
			var size = Font.MeasureString(text);
			return new Vector2(size.Width, size.Height);
		}

		public override void Print(string text, Vector2 position, Color color, float angle, Vector2 origin, Vector2 scale, SpriteEffects effects) {
			Batch.DrawString(Font, text, new Vector2(position.X + 1, position.Y - 2), 
				color, angle, origin, scale, effects, 0f);
		}
	}
}