using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class RombRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			double H = GetHeight();
			double W = GetWidth();
			int Hh = GetHeight() / 2;
			int Ww = GetWidth() / 2;
			byte S = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
			Painter.DrawLine(Level, new Point(this.Left + 2, this.Top + (int) Math.Floor(H / 2) - 1), new Point(this.Left + (int) Math.Floor(W / 2) - 1, this.Top + 2), S);
			Painter.DrawLine(Level, new Point(this.Left + 2, this.Top + (int) Math.Ceil(H / 2) + 1), new Point(this.Left + (int) Math.Floor(W / 2) - 1, this.Bottom - 2), S);
			Painter.DrawLine(Level, new Point(this.Right - 2, this.Top + (int) Math.Floor(H / 2) - 1), new Point(this.Right - (int) Math.Floor((W - 0.5) / 2) + 1, this.Top + 2), S);
			Painter.DrawLine(Level, new Point(this.Right - 2, this.Top + (int) Math.Ceil(H / 2) + 1), new Point(this.Right - (int) Math.Floor((W - 0.5) / 2) + 1, this.Bottom - 2), S);
		}

		public override bool CanConnect(Point P) {
			if (!(P.X == this.Left + this.GetWidth() / 2 && P.Y == this.Top) && !(P.X == this.Left + this.GetWidth() / 2 && P.Y == this.Bottom) && !(P.X == this.Left && P.Y == this.Top + this.GetHeight() / 2) && !(P.X == this.Right && P.Y == this.Top + this.GetHeight() / 2)) {
				return false;
			} 

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
