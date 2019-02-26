using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class MazeRoom : RegularRoom {
		private static byte[] Types = {Terrain.WALL, Terrain.CHASM};
		private static float[] Chanches = {1, 1f};

		public override void Paint(Level Level) {
			var F = Terrain.RandomFloor();
			var Wall = Types[Random.Chances(Chanches)];
			bool[][] Maze = Maze.Generate(this);
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, F);

			for (var X = 0; X < GetWidth(); X++)
			for (var Y = 0; Y < GetHeight(); Y++)
				if (Maze[X][Y] == Maze.FILLED)
					Painter.Set(Level, Left + X, Top + Y, X == 0 || Y == 0 || X == GetWidth() - 1 || Y == GetHeight() - 1 ? Terrain.WALL : Wall);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
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