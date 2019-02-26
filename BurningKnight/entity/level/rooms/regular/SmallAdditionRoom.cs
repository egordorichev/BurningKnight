using BurningKnight.entity.level.painters;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class SmallAdditionRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			Painter.DrawLine(Level, new Point(Left + GetWidth() / 2 - 2, Top + 1), new Point(Left + GetWidth() / 2 - 2, Bottom - 1), Terrain.WALL);
			Painter.DrawLine(Level, new Point(Left + GetWidth() / 2 + 2, Top + 1), new Point(Left + GetWidth() / 2 + 2, Bottom - 1), Terrain.WALL);
			Painter.Fill(Level, this, 2, Terrain.RandomFloor());
		}

		public override bool CanConnect(Point P) {
			if (P.X == Left + GetWidth() / 2 - 2 && P.Y == Top || P.X == Left + GetWidth() / 2 - 2 && P.Y == Bottom || P.X == Left + GetWidth() / 2 + 2 && P.Y == Top || P.X == Left + GetWidth() / 2 + 2 && P.Y == Bottom) return false;

			return base.CanConnect(P);
		}

		public override int GetMinWidth() {
			return 10;
		}

		public override int GetMinHeight() {
			return 10;
		}
	}
}