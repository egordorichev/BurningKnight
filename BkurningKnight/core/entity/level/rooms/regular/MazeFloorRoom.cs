using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class MazeFloorRoom : RegularRoom {
		public override Void Paint(Level Level) {
			bool[][] Maze = Maze.Generate(this);
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_B);

			for (int X = 1; X < this.GetWidth() - 1; X++) {
				for (int Y = 1; Y < this.GetHeight() - 1; Y++) {
					if (Maze[X][Y] == Maze.FILLED) {
						Painter.Set(Level, this.Left + X, this.Top + Y, Terrain.DIRT);
					} 
				}
			}

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}
	}
}
