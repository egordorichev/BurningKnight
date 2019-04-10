using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ctrings.token {
	public class TextToken : Token {
		private bool measured;
		private Vector2 size;
		
		public override Vector2 Render(CFont font, Vector2 start, Ctring ctring) {
			font.Print(Text, start, ctring.CurrentColor, 0, Vector2.Zero, ctring.CurrentScale, SpriteEffects.None);

			if (!measured) {
				
			}
			
			return start + size;
		}
	}
}