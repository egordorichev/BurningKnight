using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class CenterWallRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Random.Chance(40) ? Terrain.CHASM : Terrain.WALL;
			Painter.DrawLine(Level, new Point(Left + GetWidth() / 2 - 2, Top + GetHeight() / 2 + 1), new Point(Left + GetWidth() / 2 - 2, Top + 1), F);
			Painter.DrawLine(Level, new Point(Left + GetWidth() / 2 + 2, Top + GetHeight() / 2 - 1), new Point(Left + GetWidth() / 2 + 2, Bottom - 1), F);

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(Left + GetWidth() / 2 - 1, Top + GetHeight() / 2 + 1), F);
				Painter.Set(Level, new Point(Left + GetWidth() / 2 + 1, Top + GetHeight() / 2 - 1), F);
			}
		}

		public override bool CanConnect(Point P) {
			if (P.Y == Top || P.Y == Bottom) return false;

			return base.CanConnect(P);
		}

		public override int GetMinWidth() {
			return 10;
		}

		public override int GetMinHeight() {
			return 7;
		}
	}
}