namespace BurningKnight.entity.door {
	public class CageDoor : LockableDoor {
		protected override Lock CreateLock() {
			return new CageLock();
		}
	}
}