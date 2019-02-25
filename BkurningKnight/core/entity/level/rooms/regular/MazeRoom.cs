using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class MazeRoom : RegularRoom {
		private static byte[] Types = { Terrain.WALL, Terrain.CHASM };
		private static float[] Chanches = { 1, 1f };

		public override Void Paint(Level Level) {
			byte F = Terrain.RandomFloor();
			byte Wall = Types[Random.Chances(Chanches)];
			bool[][] Maze = Maze.Generate(this);
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, F);

			for (int X = 0; X < this.GetWidth(); X++) {
				for (int Y = 0; Y < this.GetHeight(); Y++) {
					if (Maze[X][Y] == Maze.FILLED) {
						Painter.Set(Level, this.Left + X, this.Top + Y, (X == 0 || Y == 0 || X == this.GetWidth() - 1 || Y == this.GetHeight() - 1) ? Terrain.WALL : Wall);
					} 
				}
			}

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}

		public override int GetMinWidth() {
			return 14;
		}

		public override int GetMinHeight() {
			return 14;
		}

		public override int GetMaxHeight() {
			return 19;
		}

		public override int GetMaxWidth() {
			return 19;
		}
	}
}
