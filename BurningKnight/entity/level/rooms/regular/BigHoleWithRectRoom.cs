using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class BigHoleWithRectRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 2, Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			var F = Terrain.RandomFloor();
			var W = GetWidth();
			Painter.DrawLine(Level, new Point(Left + W / 2, Top + 1), new Point(Left + W / 2, Top + GetHeight() / 2), F);
			Painter.Fill(Level, this, Math.Min(GetWidth() / 2, GetHeight() / 2) - 1, F);
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}
	}
}