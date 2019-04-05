using BurningKnight.entity.level.rooms;
using BurningKnight.util;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.entity.level.floors {
	public class MazeFloor : FloorPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			if (Random.Chance()) {
				Painter.Fill(level, inside, Tiles.RandomFloor());
				inside = inside.Shrink(Random.Int(1, 3));
			}

			var a = Tiles.RandomFloor();
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