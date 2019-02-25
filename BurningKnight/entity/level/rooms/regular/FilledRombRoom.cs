using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class FilledRombRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			double H = GetHeight();
			double W = GetWidth();
			var Hh = GetHeight() / 2;
			var Ww = GetWidth() / 2;
			var Floor = Random.Chance(33) ? Terrain.WALL : (Random.Chance(50) ? Terrain.LAVA : Terrain.CHASM);
			var Fix = Floor == Terrain.LAVA ? (Random.Chance(50) ? Terrain.WATER : Terrain.DIRT) : Terrain.RandomFloor();
			var A = Random.Chance(30);

			if (A || Random.Chance(50)) Painter.Triangle(Level, new Point(Left + 1, Top + (int) Math.Ceil(H / 2) + 1), new Point(Left + 1, Bottom - 1), new Point(Left + (int) Math.Floor(W / 2) - 1, Bottom - 1), Floor);

			if (A || Random.Chance(50)) Painter.Triangle(Level, new Point(Left + 1, Top + (int) Math.Floor(H / 2) - 1), new Point(Left + 1, Top + 1), new Point(Left + (int) Math.Floor(W / 2) - 1, Top + 1), Floor);

			if (A || Random.Chance(50)) Painter.Triangle(Level, new Point(Right - (int) Math.Floor((W - 0.5) / 2) + 1, Bottom - 1), new Point(Right - 1, Top + (int) Math.Ceil(H / 2) + 1), new Point(Right - 1, Bottom), Floor);

			if (A || Random.Chance(50)) Painter.Triangle(Level, new Point(Right - (int) Math.Floor((W - 0.5) / 2) + 1, Top + 1), new Point(Right - 1, Top + 1), new Point(Right - 1, Top + (int) Math.Floor(H / 2) - 1), Floor);

			var R = Random.NewFloat(1);
			Floor = Random.Chance(50) ? Terrain.WALL : (Random.Chance(30) ? Terrain.LAVA : Terrain.CHASM);

			if (R < 0.33f) {
				var M = Random.NewInt(2, 5) + 3;

				if (Random.Chance(50)) {
					Painter.FillEllipse(Level, this, M - Random.NewInt(1, 3), Fix);
					Painter.FillEllipse(Level, this, M + 2 + Random.NewInt(3), Floor);
				}
				else {
					Painter.Fill(Level, this, M - Random.NewInt(1, 3), Fix);
					Painter.Fill(Level, this, M, Floor);
				}
			}
			else if (R < 0.66f) {
				var M = Random.NewInt(1, 3) + 1;
				var Rect = Shrink((int) ((Ww - 0.5) / 2 + M), (int) ((Hh - 0.5) / 2 + M));

				if (Random.Chance(50))
					Painter.FillEllipse(Level, Rect, Floor);
				else
					Painter.Fill(Level, Rect, Floor);
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
			if (!(P.X == Left + GetWidth() / 2 && P.Y == Top) && !(P.X == Left + GetWidth() / 2 && P.Y == Bottom) && !(P.X == Left && P.Y == Top + GetHeight() / 2) && !(P.X == Right && P.Y == Top + GetHeight() / 2)) return false;

			return base.CanConnect(P);
		}
	}
}