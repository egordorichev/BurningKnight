namespace BurningKnight.level.rooms.special {
	public class LockedRoom : SpecialRoom {
		public override void SetupDoors(Level level) {
			base.Paint(level);
			
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Locked;
			}
		}
	}
}