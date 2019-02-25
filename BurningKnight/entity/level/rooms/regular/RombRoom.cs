using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class RombRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			double H = GetHeight();
			double W = GetWidth();
			var Hh = GetHeight() / 2;
			var Ww = GetWidth() / 2;
			var S = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
			Painter.DrawLine(Level, new Point(Left + 2, Top + (int) Math.Floor(H / 2) - 1), new Point(Left + (int) Math.Floor(W / 2) - 1, Top + 2), S);
			Painter.DrawLine(Level, new Point(Left + 2, Top + (int) Math.Ceil(H / 2) + 1), new Point(Left + (int) Math.Floor(W / 2) - 1, Bottom - 2), S);
			Painter.DrawLine(Level, new Point(Right - 2, Top + (int) Math.Floor(H / 2) - 1), new Point(Right - (int) Math.Floor((W - 0.5) / 2) + 1, Top + 2), S);
			Painter.DrawLine(Level, new Point(Right - 2, Top + (int) Math.Ceil(H / 2) + 1), new Point(Right - (int) Math.Floor((W - 0.5) / 2) + 1, Bottom - 2), S);
		}

		public override bool CanConnect(Point P) {
			if (!(P.X == Left + GetWidth() / 2 && P.Y == Top) && !(P.X == Left + GetWidth() / 2 && P.Y == Bottom) && !(P.X == Left && P.Y == Top + GetHeight() / 2) && !(P.X == Right && P.Y == Top + GetHeight() / 2)) return false;

			return base.CanConnect(P);
		}

		public override int GetMinHeight() {
			return 12;
		}

		public override int GetMinWidth() {
			return 12;
		}
	}
}