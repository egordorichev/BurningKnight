using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.treasure {
	public class MazeTreasureRoom : TreasureRoom {
		private static byte[] Types = { Terrain.WALL, Terrain.CHASM };
		private static float[] Chanches = { 1, 1f };

		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte Wall = Types[Random.Chances(Chanches)];
			bool[][] Maze = Maze.Generate(this);
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			} else {
				Painter.Fill(Level, this, 1, Terrain.RandomFloor());
				Painter.FillEllipse(Level, this, 1, Terrain.FLOOR_D);
			}


			if (Random.Chance(50)) {
				if (Random.Chance(50)) {
					Painter.Fill(Level, this, Random.NewInt(2, 5), Terrain.RandomFloor());
				} else {
					Painter.FillEllipse(Level, this, Random.NewInt(2, 5), Terrain.RandomFloor());
				}

			} 

			for (int X = 0; X < this.GetWidth(); X++) {
				for (int Y = 0; Y < this.GetHeight(); Y++) {
					if (Maze[X][Y] == Maze.FILLED) {
						Painter.Set(Level, this.Left + X, this.Top + Y, (X == 0 || Y == 0 || X == this.GetWidth() - 1 || Y == this.GetHeight() - 1) ? Terrain.WALL : Wall);
					} 
				}
			}

			Point Center = this.GetCenter();

			if (Random.Chance(50)) {
				Painter.Fill(Level, (int) Center.X - 2, (int) Center.Y - 2, 5, 5, Random.Chance(50) ? Terrain.FLOOR_B : Terrain.FLOOR_D);
			} else {
				Painter.FillEllipse(Level, (int) Center.X - 2, (int) Center.Y - 2, 5, 5, Random.Chance(50) ? Terrain.FLOOR_B : Terrain.FLOOR_D);
			}


			PlaceChest(Center);
		}

		public override int GetMinWidth() {
			return 13;
		}

		public override int GetMinHeight() {
			return 13;
		}

		public override int GetMaxHeight() {
			return 16;
		}

		public override int GetMaxWidth() {
			return 16;
		}
	}
}
