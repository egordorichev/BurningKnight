using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls {
	public class MazeWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			var w = Tiles.Pick(Tile.Chasm, Tile.Planks, Tile.WallA, Tile.Lava);
			var s = Random.Chance(40);

			var maze = Maze.Generate(room);
			
			for (int y = 1; y < maze[0].Length - 1; y++) {
				for (int x = 1; x < maze.Length - 1; x++) {
					if (maze[x][y]) {
						Painter.Set(level, x + room.Left, y + room.Top, w);
					} else if (s && Random.Chance(20)) {
						Painter.Set(level, x + room.Left, y + room.Top, Tile.SensingSpikeTmp);
					}
				}
			}
		}
	}
}