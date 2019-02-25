using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class LineMazeRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			for (int X = this.Left + 1; X < this.Right; X++) {
				if ((X - this.Left) % 2 == 0 && !Random.Chance(30)) {
					if (Random.Chance(30)) {
						Painter.DrawLine(Level, new Point(X, this.Top + 1), new Point(X, this.Bottom - 1), Terrain.RandomFloor());
						Painter.DrawLine(Level, new Point(X, this.Top + 1), new Point(X, this.Bottom - 1), Terrain.LAVA);

						for (int I = 0; I < (Random.Chance(20) ? 2 : 1); I++) {
							Painter.Set(Level, new Point(X, Random.NewInt(this.Top + 1, this.Bottom)), Random.Chance(50) ? Terrain.WATER : Terrain.DIRT);
						}
					} else {
						Painter.DrawLine(Level, new Point(X, this.Top + 1), new Point(X, this.Bottom - 1), Random.Chance(25) ? Terrain.WALL : Terrain.CHASM);

						for (int I = 0; I < (Random.Chance(20) ? 2 : 1); I++) {
							Painter.Set(Level, new Point(X, Random.NewInt(this.Top + 1, this.Bottom)), Terrain.RandomFloor());
						}
					}

				} else {
					Painter.DrawLine(Level, new Point(X, this.Top + 1), new Point(X, this.Bottom - 1), Terrain.RandomFloor());
				}

			}
		}

		public override bool CanConnect(Point P) {
			if (P.X != this.Left && P.X != this.Right && (P.X - this.Left) % 2 == 0) {
				return false;
			} 

			return base.CanConnect(P);
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W - 1;
		}
	}
}
