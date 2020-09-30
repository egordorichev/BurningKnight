using BurningKnight.level.rooms.special;
using BurningKnight.util.geometry;

namespace BurningKnight.level.rooms.payed {
	public class PayedRoom : SpecialRoom {
		public override void Paint(Level level) {
			base.Paint(level);
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Payed;
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