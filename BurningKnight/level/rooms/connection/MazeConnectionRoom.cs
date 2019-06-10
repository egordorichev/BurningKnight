using BurningKnight.level.tile;
using BurningKnight.util;

namespace BurningKnight.level.rooms.connection {
	public class MazeConnectionRoom : ConnectionRoom {
		public override void Paint(Level level) {
			var maze = Maze.Generate(this);
			var wall = Tiles.Pick(Tile.WallA, Tile.WallB, Tile.Chasm, Tile.Lava, Tile.Planks);

			for (int x = 1; x < maze.Length - 1; x++) {
				for (int y = 1; y < maze[0].Length - 1; y++) {
					if (maze[x][y]) {
						Painter.Set(level, x + Left, y + Top, wall);
					}
				}
			}
		}
	}
}