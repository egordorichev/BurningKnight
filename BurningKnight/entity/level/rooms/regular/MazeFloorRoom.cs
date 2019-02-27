using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.rooms.regular {
	public class MazeFloorRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			bool[][] Maze = Maze.Generate(this);
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_B);

			for (var X = 1; X < GetWidth() - 1; X++)
			for (var Y = 1; Y < GetHeight() - 1; Y++)
				if (Maze[X][Y] == Maze.FILLED)
					Painter.Set(Level, Left + X, Top + Y, Terrain.DIRT);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}
	}
}