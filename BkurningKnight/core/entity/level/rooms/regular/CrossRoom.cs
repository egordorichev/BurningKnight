using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class CrossRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			bool Wall = Random.Chance(50);
			Painter.Fill(Level, this, 1, Wall ? Terrain.WALL : Terrain.CHASM);

			if (Wall && Random.Chance(50)) {
				if (Random.Chance(50)) {
					Painter.Fill(Level, this, 2, Terrain.CHASM);
				} else {
					Painter.FillEllipse(Level, this, 2, Terrain.CHASM);
				}

			} 

			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, new Rect(this.Left + 1, this.Top + this.GetHeight() / 2 - 1, this.Right, this.Top + this.GetHeight() / 2 + 1), F);
			Painter.Fill(Level, new Rect(this.Left + this.GetWidth() / 2 - 1, this.Top + 1, this.Left + this.GetWidth() / 2 + 1, this.Bottom), F);

			if (!Wall && Random.Chance(50)) {
				if (Random.Chance(50)) {
					Painter.Fill(Level, this, 2, F);
				} else {
					Painter.FillEllipse(Level, this, 2, F);
				}

			} 
		}

		public override bool CanConnect(Point P) {
			if (!(P.X == this.Left + this.GetWidth() / 2 && P.Y == this.Top) && !(P.X == this.Left + this.GetWidth() / 2 && P.Y == this.Bottom) && !(P.X == this.Left && P.Y == this.Top + this.GetHeight() / 2) && !(P.X == this.Right && P.Y == this.Top + this.GetHeight() / 2) && !(P.X == this.Left + this.GetWidth() / 2 - 1 && P.Y == this.Top) && !(P.X == this.Left + this.GetWidth() / 2 - 1 && P.Y == this.Bottom) && !(P.X == this.Left && P.Y == this.Top + this.GetHeight() / 2 - 1) && !(P.X == this.Right && P.Y == this.Top + this.GetHeight() / 2 - 1)) {
				return false;
			} 

			return base.CanConnect(P);
		}

		public override int GetMinWidth() {
			return 9;
		}

		public override int GetMinHeight() {
			return 9;
		}
	}
}
