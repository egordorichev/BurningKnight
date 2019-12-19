using BurningKnight.level.rooms.special;
using BurningKnight.util.geometry;

namespace BurningKnight.level.rooms.challenge {
	public class ChallengeRoom : SpecialRoom {
		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Challenge;
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