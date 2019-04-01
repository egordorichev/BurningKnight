using BurningKnight.entity.level.entities;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.rooms.entrance {
	public class ExitRoom : EntranceRoom {
		public int To;

		public override void Paint(Level Level) {
			base.Paint(Level);
			Place(Level, GetCenter());
		}

		protected void Place(Level Level, Vector2 Point) {
			Level.Area.Add(new Exit {
				X = Point.X * 16 + 1, 
				Y = Point.Y * 16 - 6,
				To = To
			});

			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Enemy;
			}
		}
	}
}