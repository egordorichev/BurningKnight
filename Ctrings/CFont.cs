using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ctrings {
	public abstract class CFont {
		public abstract Vector2 MeasureString(string text);
		public abstract void Print(string text, Vector2 position, Color color, float angle, Vector2 origin, Vector2 scale, SpriteEffects effects);
	}
}