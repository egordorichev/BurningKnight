using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class LineRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Terrain.RandomFloor();
			var Fl = Random.Chance(30) ? Terrain.WALL : (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			Painter.Fill(Level, this, Terrain.WALL);

			if (Fl == Terrain.LAVA) F = Random.Chance(40) ? Terrain.WATER : Terrain.DIRT;

			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			var M = 2 + Random.NewInt(3);
			Painter.Fill(Level, this, M, Fl);
			Painter.Fill(Level, this, M + 1, F);

			if (Random.Chance(50)) {
				Fl = Random.Chance(30) ? Terrain.WALL : (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);

				if (Random.Chance(50))
					Painter.Fill(Level, this, M + 2 + Random.NewInt(2), Fl);
				else
					Painter.FillEllipse(Level, this, M + 2 + Random.NewInt(2), Fl);
			}

			Point Point;

			switch (Random.NewInt(4)) {
				case 0:
				default: {
					Point = new Point(GetWidth() / 2 + Left, Top + M);

					break;
				}

				case 1: {
					Point = new Point(GetWidth() / 2 + Left, Bottom - M);

					break;
				}

				case 2: {
					Point = new Point(Left + M, GetHeight() / 2 + Top);

					break;
				}

				case 3: {
					Point = new Point(Right - M, GetHeight() / 2 + Top);

					break;
				}
			}

			Painter.Set(Level, Point, F);
		}

		public override int GetMinWidth() {
			return 8;
		}

		public override int GetMinHeight() {
			return 8;
		}
	}
}