using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class LineMazeRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);

			for (var X = Left + 1; X < Right; X++)
				if ((X - Left) % 2 == 0 && !Random.Chance(30)) {
					if (Random.Chance(30)) {
						Painter.DrawLine(Level, new Point(X, Top + 1), new Point(X, Bottom - 1), Terrain.RandomFloor());
						Painter.DrawLine(Level, new Point(X, Top + 1), new Point(X, Bottom - 1), Terrain.LAVA);

						for (var I = 0; I < (Random.Chance(20) ? 2 : 1); I++) Painter.Set(Level, new Point(X, Random.NewInt(Top + 1, Bottom)), Random.Chance(50) ? Terrain.WATER : Terrain.DIRT);
					}
					else {
						Painter.DrawLine(Level, new Point(X, Top + 1), new Point(X, Bottom - 1), Random.Chance(25) ? Terrain.WALL : Terrain.CHASM);

						for (var I = 0; I < (Random.Chance(20) ? 2 : 1); I++) Painter.Set(Level, new Point(X, Random.NewInt(Top + 1, Bottom)), Terrain.RandomFloor());
					}
				}
				else {
					Painter.DrawLine(Level, new Point(X, Top + 1), new Point(X, Bottom - 1), Terrain.RandomFloor());
				}
		}

		public override bool CanConnect(Point P) {
			if (P.X != Left && P.X != Right && (P.X - Left) % 2 == 0) return false;

			return base.CanConnect(P);
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}
	}
}