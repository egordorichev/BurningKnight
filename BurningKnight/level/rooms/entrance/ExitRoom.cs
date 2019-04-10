using BurningKnight.level.entities;
using BurningKnight.level.walls;
using BurningKnight.state;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.entrance {
	public class ExitRoom : EntranceRoom {
		public int To;

		public ExitRoom() {
			To = Run.Depth + 1;
		}

		public override void Paint(Level level) {
			WallRegistry.Paint(level, this, EntranceWallPool.Instance);
			Place(level, GetCenter());
			
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Regular;
			}
		}
		
		protected void Place(Level Level, Vector2 Point) {
			Exit exit;
			
			Level.Area.Add(exit = new Exit {
				To = To
			});

			exit.Center = Point * 16;

			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Enemy;
			}
		}
	}
}