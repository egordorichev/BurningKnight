using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.rooms;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.util {
	public class Maze {
		public static bool EMPTY = false;
		public static bool FILLED = true;

		public static bool Generate(Room R) {
			bool[][] Maze = new bool[R.GetHeight()][R.GetWidth()];

			for (int X = 0; X < Maze.Length; X++) {
				for (int Y = 0; Y < Maze[0].Length; Y++) {
					if (X == 0 || X == Maze.Length - 1 || Y == 0 || Y == Maze[0].Length - 1) {
						Maze[X][Y] = FILLED;
					} 
				}
			}

			foreach (LDoor D in R.GetConnected().Values()) {
				Maze[(int) (D.X - R.Left)][(int) (D.Y - R.Top)] = EMPTY;
			}

			return Generate(Maze);
		}

		public static bool Generate(Rect R) {
			return Generate(R.GetWidth() + 1, R.GetHeight() + 1);
		}

		public static bool Generate(int Width, int Height) {
			return Generate(new bool[Height][Width]);
		}

		public static bool Generate(bool Maze) {
			int Fails = 0;
			int X;
			int Y;
			int Moves;
			int[] Mov;

			while (Fails < 2500) {
				do {
					X = Random.NewInt(Maze.Length);
					Y = Random.NewInt(Maze[0].Length);
				} while (!Maze[X][Y]);

				Mov = DecideDirection(Maze, X, Y);

				if (Mov == null) {
					Fails++;
				} else {
					Fails = 0;
					Moves = 0;

					do {
						X += Mov[0];
						Y += Mov[1];
						Maze[X][Y] = FILLED;
						Moves++;
					} while (Random.NewInt(Moves) == 0 && CheckValidMove(Maze, X, Y, Mov));
				}

			}

			return Maze;
		}

		private static int DecideDirection(bool Maze, int X, int Y) {
			if (Random.NewInt(4) == 0 && CheckValidMove(Maze, X, Y, { 0, -1 })) {
				return { 0, -1 };
			} 

			if (Random.NewInt(3) == 0 && CheckValidMove(Maze, X, Y, { 1, 0 })) {
				return { 1, 0 };
			} 

			if (Random.NewInt(2) == 0 && CheckValidMove(Maze, X, Y, { 0, 1 })) {
				return { 0, 1 };
			} 

			if (CheckValidMove(Maze, X, Y, { -1, 0 })) {
				return { -1, 0 };
			} 

			return null;
		}

		private static bool CheckValidMove(bool Maze, int X, int Y, int Mov) {
			int SideX = 1 - Math.Abs(Mov[0]);
			int SideY = 1 - Math.Abs(Mov[1]);

			for (int I = 0; I < 2; I++) {
				X += Mov[0];
				Y += Mov[1];

				if (!(X > 0 && X < Maze.Length - 1 && Y > 0 && Y < Maze[0].Length - 1 && !Maze[X][Y] && !Maze[X + SideX][Y + SideY] && !Maze[X - SideX][Y - SideY])) {
					return false;
				} 
			}

			return true;
		}
	}
}
