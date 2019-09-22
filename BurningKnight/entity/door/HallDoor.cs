namespace BurningKnight.entity.door {
	public class HallDoor : LockableDoor {
		protected override Lock CreateLock() {
			return new HallLock();
		}
	}
}