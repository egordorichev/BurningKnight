namespace BurningKnight.entity.door {
	public class SpecialDoor : LockableDoor {
		protected override Lock CreateLock() {
			return new GoldLock();
		}
	}
}