namespace BurningKnight.level.rooms.special {
	public class LockedRoom : SpecialRoom {
		public override void SetupDoors() {
			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Locked;
			}
		}
	}
}