using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class VerticalMazeRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);

			for (var Y = Top + 1; Y < Bottom; Y++)
				if ((Y - Top) % 2 == 0 && !Random.Chance(30)) {
					if (Random.Chance(30)) {
						Painter.DrawLine(Level, new Point(Left + 1, Y), new Point(Right - 1, Y), Terrain.RandomFloor());
						Painter.DrawLine(Level, new Point(Left + 1, Y), new Point(Right - 1, Y), Terrain.LAVA);

						for (var I = 0; I < (Random.Chance(20) ? 2 : 1); I++) Painter.Set(Level, new Point(Random.NewInt(Left + 1, Right), Y), Random.Chance(50) ? Terrain.WATER : Terrain.DIRT);
					}
					else {
						Painter.DrawLine(Level, new Point(Left + 1, Y), new Point(Right - 1, Y), Random.Chance(25) ? Terrain.WALL : Terrain.CHASM);

						for (var I = 0; I < (Random.Chance(20) ? 2 : 1); I++) Painter.Set(Level, new Point(Random.NewInt(Left + 1, Right), Y), Terrain.RandomFloor());
					}
				}
				else {
					Painter.DrawLine(Level, new Point(Left + 1, Y), new Point(Right - 1, Y), Terrain.RandomFloor());
				}
		}

		public override bool CanConnect(Point P) {
			if (P.Y != Top && P.Y != Bottom && (P.Y - Top) % 2 == 0) return false;

			return base.CanConnect(P);
		}

		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H - 1;
		}
	}
}