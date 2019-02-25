using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class SpikedRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (this.GetWidth() > 5 && this.GetHeight() > 5) {
				Painter.Fill(Level, this, 2, Terrain.LAVA);
				Painter.Fill(Level, this, 3, Terrain.DIRT);
				Painter.Set(Level, this.Left + this.GetWidth() / 2, Random.Chance(50) ? this.Top + 2 : this.Bottom - 2, Terrain.DIRT);
			} 
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 10;
		}
	}
}
