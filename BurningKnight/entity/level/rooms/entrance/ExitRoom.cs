using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.walls;
using BurningKnight.state;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.rooms.entrance {
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
			Level.Area.Add(new Exit {
				Center = Point * 16 + new Vector2(8, 8), 
				To = To
			});

			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Enemy;
			}
		}
	}
}