using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class SmallMazeRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, 1, F);
			Painter.DrawLine(Level, new Point(this.Left + 4, this.Top + 1), new Point(this.Left + 4, this.Bottom - 1), Terrain.WALL);
			Painter.DrawLine(Level, new Point(this.Right - 4, this.Top + 1), new Point(this.Right - 4, this.Bottom - 1), Terrain.WALL);
			Painter.DrawLine(Level, new Point(this.Left + 1, this.Top + 4), new Point(this.Right - 1, this.Top + 4), Terrain.WALL);
			Painter.DrawLine(Level, new Point(this.Left + 1, this.Bottom - 4), new Point(this.Right - 1, this.Bottom - 4), Terrain.WALL);

			if (Random.Chance(30)) {
				bool[][] Maze = Maze.Generate(this);

				for (int X = 1; X < GetWidth() - 1; X++) {
					for (int Y = 1; Y < GetHeight() - 1; Y++) {
						if (Maze[X][Y] == Maze.EMPTY) {
							Painter.Set(Level, new Point(this.Left + X, this.Top + Y), F);
						} 
					}
				}
			} else {
				if (Random.Chance(50)) {
					Painter.DrawLine(Level, new Point(this.Left + 1, this.Top + 2), new Point(this.Right - 1, this.Top + 2), F);
					Painter.DrawLine(Level, new Point(this.Left + 1, this.Top + this.GetHeight() / 2), new Point(this.Right - 1, this.Top + this.GetHeight() / 2), F);
					Painter.DrawLine(Level, new Point(this.Left + 1, this.Bottom - 2), new Point(this.Right - 1, this.Bottom - 2), F);

					switch (Random.NewInt(3)) {
						case 0: {
							Painter.DrawLine(Level, new Point(this.Right - 2, this.Top + 1), new Point(this.Right - 2, this.Bottom - 1), F);

							break;
						}

						case 1: {
							Painter.DrawLine(Level, new Point(this.Left + this.GetWidth() / 2, this.Top + 1), new Point(this.Left + this.GetWidth() / 2, this.Bottom - 1), F);

							break;
						}

						case 2: {
							Painter.DrawLine(Level, new Point(this.Left + 2, this.Top + 1), new Point(this.Left + 2, this.Bottom - 1), F);

							break;
						}
					}
				} else {
					Painter.DrawLine(Level, new Point(this.Left + 2, this.Top + 1), new Point(this.Left + 2, this.Bottom - 1), F);
					Painter.DrawLine(Level, new Point(this.Left + this.GetWidth() / 2, this.Top + 1), new Point(this.Left + this.GetWidth() / 2, this.Bottom - 1), F);
					Painter.DrawLine(Level, new Point(this.Right - 2, this.Top + 1), new Point(this.Right - 2, this.Bottom - 1), F);

					switch (Random.NewInt(3)) {
						case 0: {
							Painter.DrawLine(Level, new Point(this.Left + 1, this.Top + 2), new Point(this.Right - 1, this.Top + 2), F);

							break;
						}

						case 1: {
							Painter.DrawLine(Level, new Point(this.Left + 1, this.Top + this.GetHeight() / 2), new Point(this.Right - 1, this.Top + this.GetHeight() / 2), F);

							break;
						}

						case 2: {
							Painter.DrawLine(Level, new Point(this.Left + 1, this.Bottom - 2), new Point(this.Right - 1, this.Bottom - 2), F);

							break;
						}
					}
				}

			}

		}

		public override bool CanConnect(Point P) {
			if (!(P.X == this.Left + 2 && P.Y == this.Top) && !(P.X == this.Left && P.Y == this.Top + 2) && !(P.X == this.Right - 2 && P.Y == this.Top) && !(P.X == this.Right && P.Y == this.Top + 2) && !(P.X == this.Left + 2 && P.Y == this.Bottom) && !(P.X == this.Left && P.Y == this.Bottom - 2) && !(P.X == this.Right - 2 && P.Y == this.Bottom) && !(P.X == this.Right && P.Y == this.Bottom - 2)) {
				return false;
			} 

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
