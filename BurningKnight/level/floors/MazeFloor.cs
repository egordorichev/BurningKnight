using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.floors {
	public class MazeFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside, bool gold) {
			if (Rnd.Chance()) {
				Painter.Fill(level, inside, Tiles.RandomFloor());
				inside = inside.Shrink(Rnd.Int(1, 3));
			}

			var a = gold ? Tile.FloorD : Tiles.RandomFloor();
			var b = Tiles.RandomNewFloor();

			var maze = Maze.Generate(inside);
			
			for (int y = 0; y < inside.GetHeight(); y++) {
				for (int x = 0; x < inside.GetWidth(); x++) {
					Painter.Set(level, x + inside.Left, y + inside.Top, maze[x][y] ? a : b);
				}
			}
		}
	}
}