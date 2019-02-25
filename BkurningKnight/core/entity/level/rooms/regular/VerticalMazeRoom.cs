using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class VerticalMazeRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			for (int Y = this.Top + 1; Y < this.Bottom; Y++) {
				if ((Y - this.Top) % 2 == 0 && !Random.Chance(30)) {
					if (Random.Chance(30)) {
						Painter.DrawLine(Level, new Point(this.Left + 1, Y), new Point(this.Right - 1, Y), Terrain.RandomFloor());
						Painter.DrawLine(Level, new Point(this.Left + 1, Y), new Point(this.Right - 1, Y), Terrain.LAVA);

						for (int I = 0; I < (Random.Chance(20) ? 2 : 1); I++) {
							Painter.Set(Level, new Point(Random.NewInt(this.Left + 1, this.Right), Y), Random.Chance(50) ? Terrain.WATER : Terrain.DIRT);
						}
					} else {
						Painter.DrawLine(Level, new Point(this.Left + 1, Y), new Point(this.Right - 1, Y), Random.Chance(25) ? Terrain.WALL : Terrain.CHASM);

						for (int I = 0; I < (Random.Chance(20) ? 2 : 1); I++) {
							Painter.Set(Level, new Point(Random.NewInt(this.Left + 1, this.Right), Y), Terrain.RandomFloor());
						}
					}

				} else {
					Painter.DrawLine(Level, new Point(this.Left + 1, Y), new Point(this.Right - 1, Y), Terrain.RandomFloor());
				}

			}
		}

		public override bool CanConnect(Point P) {
			if (P.Y != this.Top && P.Y != this.Bottom && (P.Y - this.Top) % 2 == 0) {
				return false;
			} 

			return base.CanConnect(P);
		}

		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H - 1;
		}
	}
}
