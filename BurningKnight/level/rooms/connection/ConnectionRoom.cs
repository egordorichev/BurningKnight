using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class ConnectionRoom : RoomDef {
		public override int GetMinWidth() {
			return 3;
		}

		public override int GetMinHeight() {
			return 3;
		}

		public override int GetMaxWidth() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 10;
		}

		public override void Paint(Level level) {
			Painter.Rect(level, Left, Top, GetWidth() - 1, GetHeight() - 1, Tile.WallA);
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 16;

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 2;

			return 0;
		}
	}
}