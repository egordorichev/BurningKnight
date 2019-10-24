namespace BurningKnight.entity.door {
	public class LockedDoor : LockableDoor {
		protected override Lock CreateLock() {
			return new GoldLock();
		}
	}
}