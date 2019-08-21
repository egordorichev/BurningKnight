using BurningKnight.level.tile;
using BurningKnight.util;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class MazeConnectionRoom : ConnectionRoom {
		public override void Paint(Level level) {
			Painter.Rect(level, Left, Top, GetWidth() - 1, GetHeight() - 1, Tile.WallA);
			
			var maze = Maze.Generate(this);
			var wall = Tiles.Pick(Tile.WallA, Tile.Chasm, Tile.Lava, Tile.Planks);
			var spikes = Random.Chance(30);

			for (int x = 1; x < maze.Length - 1; x++) {
				for (int y = 1; y < maze[0].Length - 1; y++) {
					if (maze[x][y]) {
						Painter.Set(level, x + Left, y + Top, wall);
					} else if (spikes && Random.Chance(20)) {
						Painter.Set(level, x + Left, y + Top, Tile.SensingSpikeTmp);
					}
				}
			}
		}
	}
}