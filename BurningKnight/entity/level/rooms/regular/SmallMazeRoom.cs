using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class SmallMazeRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, 1, F);
			Painter.DrawLine(Level, new Point(Left + 4, Top + 1), new Point(Left + 4, Bottom - 1), Terrain.WALL);
			Painter.DrawLine(Level, new Point(Right - 4, Top + 1), new Point(Right - 4, Bottom - 1), Terrain.WALL);
			Painter.DrawLine(Level, new Point(Left + 1, Top + 4), new Point(Right - 1, Top + 4), Terrain.WALL);
			Painter.DrawLine(Level, new Point(Left + 1, Bottom - 4), new Point(Right - 1, Bottom - 4), Terrain.WALL);

			if (Random.Chance(30)) {
				bool[][] Maze = Maze.Generate(this);

				for (var X = 1; X < GetWidth() - 1; X++)
				for (var Y = 1; Y < GetHeight() - 1; Y++)
					if (Maze[X][Y] == Maze.EMPTY)
						Painter.Set(Level, new Point(Left + X, Top + Y), F);
			}
			else {
				if (Random.Chance(50)) {
					Painter.DrawLine(Level, new Point(Left + 1, Top + 2), new Point(Right - 1, Top + 2), F);
					Painter.DrawLine(Level, new Point(Left + 1, Top + GetHeight() / 2), new Point(Right - 1, Top + GetHeight() / 2), F);
					Painter.DrawLine(Level, new Point(Left + 1, Bottom - 2), new Point(Right - 1, Bottom - 2), F);

					switch (Random.NewInt(3)) {
						case 0: {
							Painter.DrawLine(Level, new Point(Right - 2, Top + 1), new Point(Right - 2, Bottom - 1), F);

							break;
						}

						case 1: {
							Painter.DrawLine(Level, new Point(Left + GetWidth() / 2, Top + 1), new Point(Left + GetWidth() / 2, Bottom - 1), F);

							break;
						}

						case 2: {
							Painter.DrawLine(Level, new Point(Left + 2, Top + 1), new Point(Left + 2, Bottom - 1), F);

							break;
						}
					}
				}
				else {
					Painter.DrawLine(Level, new Point(Left + 2, Top + 1), new Point(Left + 2, Bottom - 1), F);
					Painter.DrawLine(Level, new Point(Left + GetWidth() / 2, Top + 1), new Point(Left + GetWidth() / 2, Bottom - 1), F);
					Painter.DrawLine(Level, new Point(Right - 2, Top + 1), new Point(Right - 2, Bottom - 1), F);

					switch (Random.NewInt(3)) {
						case 0: {
							Painter.DrawLine(Level, new Point(Left + 1, Top + 2), new Point(Right - 1, Top + 2), F);

							break;
						}

						case 1: {
							Painter.DrawLine(Level, new Point(Left + 1, Top + GetHeight() / 2), new Point(Right - 1, Top + GetHeight() / 2), F);

							break;
						}

						case 2: {
							Painter.DrawLine(Level, new Point(Left + 1, Bottom - 2), new Point(Right - 1, Bottom - 2), F);

							break;
						}
					}
				}
			}
		}

		public override bool CanConnect(Point P) {
			if (!(P.X == Left + 2 && P.Y == Top) && !(P.X == Left && P.Y == Top + 2) && !(P.X == Right - 2 && P.Y == Top) && !(P.X == Right && P.Y == Top + 2) && !(P.X == Left + 2 && P.Y == Bottom) && !(P.X == Left && P.Y == Bottom - 2) &&
			    !(P.X == Right - 2 && P.Y == Bottom) && !(P.X == Right && P.Y == Bottom - 2)) return false;

			return base.CanConnect(P);
		}

		public override int GetMinWidth() {
			return 13;
		}

		public override int GetMinHeight() {
			return 13;
		}
	}
}