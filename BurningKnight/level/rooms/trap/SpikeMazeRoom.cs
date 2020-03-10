using BurningKnight.entity.room;
using BurningKnight.entity.room.input;
using BurningKnight.level.tile;
using BurningKnight.util;

namespace BurningKnight.level.rooms.trap {
	public class SpikeMazeRoom : TrapRoom {
		public override void ModifyRoom(Room room) {
			base.ModifyRoom(room);
			room.AddController("bk:trap");
		}

		public override void Paint(Level level) {
			base.Paint(level);
			var maze = Maze.Generate(this);
			
			for (int y = 1; y < maze[0].Length - 1; y++) {
				for (int x = 1; x < maze.Length - 1; x++) {
					if (maze[x][y]) {
						Painter.Set(level, x + Left, y + Top, Tile.SpikeOnTmp);
					}
				}
			}

			var where = GetTileCenter();
			Painter.Set(level, where, Tiles.RandomFloor());

			level.Area.Add(new Button {
				Position = where * 16
			});
		}
	}
}