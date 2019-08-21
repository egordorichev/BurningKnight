using BurningKnight.entity.room.controllable.spikes;
using BurningKnight.level.tile;

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

		public void CoverInSpikes(Level level) {
			for (var y = Top + 2; y < Bottom - 1; y++) {
				for (var x = Left + 2; x < Right - 1; x++) {
					var t = level.Get(x, y);

					if (t.IsWall() || t == Tile.Chasm || t == Tile.PistonDown) {
						continue;
					}

					var spikes = new SensingSpikes();

					spikes.X = x * 16;
					spikes.Y = y * 16;

					level.Area.Add(spikes);
				}
			}
		}
	}
}