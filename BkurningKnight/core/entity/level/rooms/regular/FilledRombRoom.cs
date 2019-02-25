using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class FilledRombRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			double H = GetHeight();
			double W = GetWidth();
			int Hh = GetHeight() / 2;
			int Ww = GetWidth() / 2;
			byte Floor = Random.Chance(33) ? Terrain.WALL : (Random.Chance(50) ? Terrain.LAVA : Terrain.CHASM);
			byte Fix = (Floor == Terrain.LAVA ? (Random.Chance(50) ? Terrain.WATER : Terrain.DIRT) : Terrain.RandomFloor());
			bool A = Random.Chance(30);

			if (A || Random.Chance(50)) {
				Painter.Triangle(Level, new Point(this.Left + 1, this.Top + (int) Math.Ceil(H / 2) + 1), new Point(this.Left + 1, this.Bottom - 1), new Point(this.Left + (int) Math.Floor(W / 2) - 1, this.Bottom - 1), Floor);
			} 

			if (A || Random.Chance(50)) {
				Painter.Triangle(Level, new Point(this.Left + 1, this.Top + (int) Math.Floor(H / 2) - 1), new Point(this.Left + 1, this.Top + 1), new Point(this.Left + (int) Math.Floor(W / 2) - 1, this.Top + 1), Floor);
			} 

			if (A || Random.Chance(50)) {
				Painter.Triangle(Level, new Point(this.Right - (int) Math.Floor((W - 0.5) / 2) + 1, this.Bottom - 1), new Point(this.Right - 1, this.Top + (int) Math.Ceil(H / 2) + 1), new Point(this.Right - 1, this.Bottom), Floor);
			} 

			if (A || Random.Chance(50)) {
				Painter.Triangle(Level, new Point(this.Right - (int) Math.Floor((W - 0.5) / 2) + 1, this.Top + 1), new Point(this.Right - 1, this.Top + 1), new Point(this.Right - 1, this.Top + (int) Math.Floor(H / 2) - 1), Floor);
			} 

			float R = Random.NewFloat(1);
			Floor = Random.Chance(50) ? Terrain.WALL : (Random.Chance(30) ? Terrain.LAVA : Terrain.CHASM);

			if (R < 0.33f) {
				int M = Random.NewInt(2, 5) + 3;

				if (Random.Chance(50)) {
					Painter.FillEllipse(Level, this, M - Random.NewInt(1, 3), Fix);
					Painter.FillEllipse(Level, this, M + 2 + Random.NewInt(3), Floor);
				} else {
					Painter.Fill(Level, this, M - Random.NewInt(1, 3), Fix);
					Painter.Fill(Level, this, M, Floor);
				}

			} else if (R < 0.66f) {
				int M = Random.NewInt(1, 3) + 1;
				Rect Rect = this.Shrink((int) ((Ww - 0.5) / 2 + M), (int) ((Hh - 0.5) / 2 + M));

				if (Random.Chance(50)) {
					Painter.FillEllipse(Level, Rect, Floor);
				} else {
					Painter.Fill(Level, Rect, Floor);
				}

			} 
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}

		public override int GetMinHeight() {
			return 10;
		}

		public override int GetMinWidth() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 24;
		}

		public override int GetMaxWidth() {
			return 24;
		}

		public override bool CanConnect(Point P) {
			if (!(P.X == this.Left + this.GetWidth() / 2 && P.Y == this.Top) && !(P.X == this.Left + this.GetWidth() / 2 && P.Y == this.Bottom) && !(P.X == this.Left && P.Y == this.Top + this.GetHeight() / 2) && !(P.X == this.Right && P.Y == this.Top + this.GetHeight() / 2)) {
				return false;
			} 

			return base.CanConnect(P);
		}
	}
}
