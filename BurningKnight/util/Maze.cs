using System;
using BurningKnight.level.rooms;
using BurningKnight.util.geometry;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.util {
	public class Maze {
		public const bool Empty = false;
		public const bool Filled = true;

		public static bool[][] Generate(RoomDef R) {
			var Maze = new bool[R.GetWidth()][];

			for (var X = 0; X < Maze.Length; X++) {
				Maze[X] = new bool[R.GetHeight()];

				for (var Y = 0; Y < Maze[0].Length; Y++) {
					if (X == 0 || X == Maze.Length - 1 || Y == 0 || Y == Maze[0].Length - 1) {
						Maze[X][Y] = Filled;
					}
				}
			}

			foreach (var D in R.Connected.Values) {
				Maze[D.X - R.Left][D.Y - R.Top] = Empty;
			}

			return Generate(Maze);
		}

		public static bool[][] Generate(Rect R) {
			return Generate(R.GetWidth() + 1, R.GetHeight() + 1);
		}

		public static bool[][] Generate(int Width, int Height) {
			var array = new bool[Width][];

			for (var X = 0; X < Width; X++) {
				array[X] = new bool[Height];

				for (var Y = 0; Y < Height; Y++) {
					if (X == 0 || X == Width - 1 || Y == 0 || Y == Height - 1) {
						array[X][Y] = Filled;
					}
				}
			}

			return Generate(array);
		}

		public static bool[][] Generate(bool[][] Maze) {
			var Fails = 0;
			int X;
			int Y;
			int Moves;
			int[] Mov;

			Log.Debug($"Generating maze {Maze.Length}x{Maze[0].Length}");

			while (Fails < 2500) {
				do {
					X = Rnd.Int(Maze.Length);
					Y = Rnd.Int(Maze[0].Length);
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
						Maze[X][Y] = Filled;
						Moves++;
					} while (Rnd.Int(Moves) == 0 && CheckValidMove(Maze, X, Y, Mov));
				}
			}

			return Maze;
		}

		private static int[] DecideDirection(bool[][] Maze, int X, int Y) {
			if (Rnd.Int(4) == 0 && CheckValidMove(Maze, X, Y, new[] {0, -1})) {
				return new[] {0, -1};
			}

			if (Rnd.Int(3) == 0 && CheckValidMove(Maze, X, Y, new[] {1, 0})) {
				return new[] {1, 0};
			}

			if (Rnd.Int(2) == 0 && CheckValidMove(Maze, X, Y, new[] {
				0, 1
			})) {
				return new[] {
					0, 1
				};
			}

			if (CheckValidMove(Maze, X, Y, new[] {
				-1, 0
			})) {
				return new[] {
					-1, 0
				};
			}

			return null;
		}

		private static bool CheckValidMove(bool[][] Maze, int X, int Y, int[] Mov) {
			var SideX = 1 - Math.Abs(Mov[0]);
			var SideY = 1 - Math.Abs(Mov[1]);

			for (var I = 0; I < 2; I++) {
				X += Mov[0];
				Y += Mov[1];

				if (!(X > 0 && X < Maze.Length - 1 && Y > 0 && Y < Maze[0].Length - 1
				      && !Maze[X][Y] && !Maze[X + SideX][Y + SideY] && !Maze[X - SideX][Y - SideY])) {
					return false;
				}
			}

			return true;
		}
	}
}