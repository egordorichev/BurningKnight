using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.entrance {
	public class MazeEntranceRoom : EntranceRoom {
		public override Void Paint(Level Level) {
			byte Wall = Random.Chance(50) ? Terrain.CHASM : Terrain.WALL;
			bool[][] Maze = Maze.Generate(this);
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			bool Set = false;

			while (!Set) {
				for (int X = 0; X < this.GetWidth(); X++) {
					for (int Y = 0; Y < this.GetHeight(); Y++) {
						if (Maze[X][Y] == Maze.FILLED) {
							Painter.Set(Level, this.Left + X, this.Top + Y, (X == 0 || Y == 0 || X == this.GetWidth() - 1 || Y == this.GetHeight() - 1) ? Terrain.WALL : Wall);
						} else if (X != 0 && Y != 0 && X != GetWidth() - 1 && Y != GetHeight() - 1 && !Set && Random.Chance(this.GetMaxWidth() * this.GetHeight() / 100)) {
							Set = true;
							Place(Level, new Point(this.Left + X, this.Top + Y));
						} 
					}
				}
			}

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}

		public override int GetMinWidth() {
			return 8;
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 16;
		}

		public override int GetMaxWidth() {
			return 16;
		}
	}
}
