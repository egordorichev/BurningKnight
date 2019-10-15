using BurningKnight.level.tile;
using BurningKnight.util;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class MazeConnectionRoom : ConnectionRoom {
		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}
		
		public override void Paint(Level level) {
			var maze = Maze.Generate(this);
			var wall = Tiles.Pick(Tile.WallA, Tile.Chasm, Tile.Lava, Tile.Planks);
			var spikes = Random.Chance(30);

			if (Random.Chance()) {
				var v = Random.Chance();
				var k = Random.Int(1, 4);
				
				for (var i = 0; i < k; i++) {
					var nmaze = new bool[maze.Length][];

					for (var j = 0; j < maze.Length; j++) {
						nmaze[j] = new bool[maze[0].Length];
					}

					for (int x = 0; x < maze.Length; x++) {
						for (int y = 0; y < maze[0].Length; y++) {
							nmaze[x][y] = maze[x][y];
						}
					}

					for (int x = 1; x < maze.Length - 1; x++) {
						for (int y = 1; y < maze[0].Length - 1; y++) {
							if (maze[x][y] == v) {
								var n = 0;

								foreach (var d in MathUtils.Directions) {
									var xx = x + (int) d.X;
									var yy = y + (int) d.Y;

									if (maze[xx][yy] == v) {
										n++;
									}
								}

								if (n < 2) {
									nmaze[x][y] = !v;
								}
							}
						}
					}

					maze = nmaze;
				}
			}

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