using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class SpikedRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);

			if (GetWidth() > 5 && GetHeight() > 5) {
				Painter.Fill(Level, this, 2, Terrain.LAVA);
				Painter.Fill(Level, this, 3, Terrain.DIRT);
				Painter.Set(Level, Left + GetWidth() / 2, Random.Chance(50) ? Top + 2 : Bottom - 2, Terrain.DIRT);
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