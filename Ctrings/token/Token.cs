using Microsoft.Xna.Framework;

namespace Ctrings.token {
	public abstract class Token {
		public string Text;
		public abstract Vector2 Render(CFont font, Vector2 start, Ctring ctring);
	}
}