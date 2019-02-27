using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.rooms.regular {
	public class RegularRoomDef : RoomDef {
		public override void Paint(Level Level) {
			Painter.Fill(Level, this, Tile.Wall);
			Painter.Fill(Level, this, 1, Tiles.RandomFloor());

			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Enemy;
			}		
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) {
				return 16;
			}

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) {
				return 1;
			}

			return 0;
		}
	}
}