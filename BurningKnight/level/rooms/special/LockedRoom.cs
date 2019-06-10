namespace BurningKnight.level.rooms.special {
	public class LockedRoom : SpecialRoom {
		public override void SetupDoors(Level level) {
			level.ItemsToSpawn.Add("bk:key");
			
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Locked;
			}
		}
	}
}