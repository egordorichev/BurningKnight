using BurningKnight.level.rooms.special;
using BurningKnight.util.geometry;

namespace BurningKnight.level.rooms.scourged {
	public class ScourgedRoom : SpecialRoom {
		public override void Paint(Level level) {
		
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Scourged;
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