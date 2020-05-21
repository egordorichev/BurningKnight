using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.mob.boss.rooms {
	public class DmMazeRoom : DmRoom {
		public override void PlaceMage(Room room, DM mage) {
			mage.BottomCenter = room.GetRandomFreeTile() * 16 + new Vector2(8);
		}

		public override void PlacePlayer(Room room, Player player) {
			player.BottomCenter = room.GetRandomFreeTile() * 16 + new Vector2(8);
		}

		public override void Paint(Level level, Room room) {
			var maze = Maze.Generate(this);
			var wall = Tile.WallA;

			for (int x = 1; x < maze.Length - 1; x++) {
				for (int y = 1; y < maze[0].Length - 1; y++) {
					if (maze[x][y]) {
						Painter.Set(level, x + Left, y + Top, wall);
					}
				}
			}
		}

		public override int GetMinWidth() {
			return 30;
		}

		public override int GetMinHeight() {
			return 30;
		}

		public override int GetMaxWidth() {
			return 20;
		}

		public override int GetMaxHeight() {
			return 20;
		}
	}
}