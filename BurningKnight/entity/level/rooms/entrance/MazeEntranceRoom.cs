using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.entrance {
	public class MazeEntranceRoom : EntranceRoom {
		public override void Paint(Level Level) {
			var Wall = Random.Chance(50) ? Terrain.CHASM : Terrain.WALL;
			bool[][] Maze = Maze.Generate(this);
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			var Set = false;

			while (!Set)
				for (var X = 0; X < GetWidth(); X++)
				for (var Y = 0; Y < GetHeight(); Y++)
					if (Maze[X][Y] == Maze.FILLED) {
						Painter.Set(Level, Left + X, Top + Y, X == 0 || Y == 0 || X == GetWidth() - 1 || Y == GetHeight() - 1 ? Terrain.WALL : Wall);
					}
					else if (X != 0 && Y != 0 && X != GetWidth() - 1 && Y != GetHeight() - 1 && !Set && Random.Chance(GetMaxWidth() * GetHeight() / 100)) {
						Set = true;
						Place(Level, new Point(Left + X, Top + Y));
					}

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
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