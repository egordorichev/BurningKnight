using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;

namespace BurningKnight.level.rooms.spiked {
	public class SpikedRoom : SpecialRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.EvilWall);
			Painter.Fill(level, this, 2, Tile.EvilFloor);
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Spiked;
			}
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right) {
				return false;
			}
			
			return base.CanConnect(R, P);
		}
	}
}