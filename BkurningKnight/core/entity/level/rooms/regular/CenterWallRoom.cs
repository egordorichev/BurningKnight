using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class CenterWallRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F = Random.Chance(40) ? Terrain.CHASM : Terrain.WALL;
			Painter.DrawLine(Level, new Point(this.Left + this.GetWidth() / 2 - 2, this.Top + this.GetHeight() / 2 + 1), new Point(this.Left + this.GetWidth() / 2 - 2, this.Top + 1), F);
			Painter.DrawLine(Level, new Point(this.Left + this.GetWidth() / 2 + 2, this.Top + this.GetHeight() / 2 - 1), new Point(this.Left + this.GetWidth() / 2 + 2, this.Bottom - 1), F);

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(this.Left + this.GetWidth() / 2 - 1, this.Top + this.GetHeight() / 2 + 1), F);
				Painter.Set(Level, new Point(this.Left + this.GetWidth() / 2 + 1, this.Top + this.GetHeight() / 2 - 1), F);
			} 
		}

		public override bool CanConnect(Point P) {
			if (P.Y == this.Top || P.Y == this.Bottom) {
				return false;
			} 

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
