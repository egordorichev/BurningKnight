using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class LineRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F = Terrain.RandomFloor();
			byte Fl = Random.Chance(30) ? Terrain.WALL : (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			Painter.Fill(Level, this, Terrain.WALL);

			if (Fl == Terrain.LAVA) {
				F = Random.Chance(40) ? Terrain.WATER : Terrain.DIRT;
			} 

			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			int M = 2 + Random.NewInt(3);
			Painter.Fill(Level, this, M, Fl);
			Painter.Fill(Level, this, M + 1, F);

			if (Random.Chance(50)) {
				Fl = Random.Chance(30) ? Terrain.WALL : (Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);

				if (Random.Chance(50)) {
					Painter.Fill(Level, this, M + 2 + Random.NewInt(2), Fl);
				} else {
					Painter.FillEllipse(Level, this, M + 2 + Random.NewInt(2), Fl);
				}

			} 

			Point Point;

			switch (Random.NewInt(4)) {
				case 0: 
				default:{
					Point = new Point(this.GetWidth() / 2 + this.Left, this.Top + M);

					break;
				}

				case 1: {
					Point = new Point(this.GetWidth() / 2 + this.Left, this.Bottom - M);

					break;
				}

				case 2: {
					Point = new Point(this.Left + M, this.GetHeight() / 2 + this.Top);

					break;
				}

				case 3: {
					Point = new Point(this.Right - M, this.GetHeight() / 2 + this.Top);

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
