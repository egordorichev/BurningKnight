using BurningKnight.level.entities;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.secret {
	public class SecretDarkMarketEntranceRoom : SecretRoom {
		public override void Paint(Level level) {
			base.Paint(level);
			
			var entrance = new HiddenEntrance();
			level.Area.Add(entrance);
			entrance.Position = new Vector2(Left + (int) ((GetWidth() - 1) / 2f), Top + (int) ((GetHeight() - 1) / 2f)) * 16;
		}

		protected override bool Quad() {
			return true;
		}
		
		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}
		
		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H - 1;
		}
	}
}