using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class BigHoleWithRectRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 2, Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			byte F = Terrain.RandomFloor();
			int W = this.GetWidth();
			Painter.DrawLine(Level, new Point(this.Left + W / 2, this.Top + 1), new Point(this.Left + W / 2, this.Top + this.GetHeight() / 2), F);
			Painter.Fill(Level, this, Math.Min(this.GetWidth() / 2, this.GetHeight() / 2) - 1, F);
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}
	}
}
