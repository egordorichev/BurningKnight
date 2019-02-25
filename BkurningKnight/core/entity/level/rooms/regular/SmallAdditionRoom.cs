using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class SmallAdditionRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.DrawLine(Level, new Point(this.Left + this.GetWidth() / 2 - 2, this.Top + 1), new Point(this.Left + this.GetWidth() / 2 - 2, this.Bottom - 1), Terrain.WALL);
			Painter.DrawLine(Level, new Point(this.Left + this.GetWidth() / 2 + 2, this.Top + 1), new Point(this.Left + this.GetWidth() / 2 + 2, this.Bottom - 1), Terrain.WALL);
			Painter.Fill(Level, this, 2, Terrain.RandomFloor());
		}

		public override bool CanConnect(Point P) {
			if ((P.X == this.Left + this.GetWidth() / 2 - 2 && P.Y == this.Top) || (P.X == this.Left + this.GetWidth() / 2 - 2 && P.Y == this.Bottom) || (P.X == this.Left + this.GetWidth() / 2 + 2 && P.Y == this.Top) || (P.X == this.Left + this.GetWidth() / 2 + 2 && P.Y == this.Bottom)) {
				return false;
			} 

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
