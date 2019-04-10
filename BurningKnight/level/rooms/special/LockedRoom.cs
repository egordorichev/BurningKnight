namespace BurningKnight.level.rooms.special {
	public class LockedRoom : SpecialRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);

			foreach (var Door in Connected.Values) {
				Door.Type = DoorPlaceholder.Variant.Locked;
			}
		}
	}
}