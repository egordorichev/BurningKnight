using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.painters;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.rooms.entrance {
	public class EntranceRoomDef : RoomDef {
		public bool Exit;

		public override bool CanConnect(RoomDef R) {
			return base.CanConnect(R) && !(R is EntranceRoomDef);
		}
		
		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;

			return 0;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 16;

			return 4;
		}

		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.Fill(Level, this, 2, Tiles.RandomFloor());
			else
				Painter.FillEllipse(Level, this, 2, Tiles.RandomFloor());


			Place(Level, GetCenter());
		}

		protected void Place(Level Level, Vector2 Point) {
			if (this.Exit) {
				var Exit = new Portal();
				Exit.X = Point.X * 16;
				Exit.Y = Point.Y * 16;
				Level.Area.Add(Exit);
			}	else {
				var Entrance = new Entrance();
				Entrance.X = Point.X * 16 + 1;
				Entrance.Y = Point.Y * 16 - 6;
				Level.Area.Add(Entrance);
			}

			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Enemy;
			}
		}
	}
}