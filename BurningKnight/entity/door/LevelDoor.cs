namespace BurningKnight.entity.door {
	public class LevelDoor : LockableDoor {
		protected override Lock CreateLock() {
			return new LevelLock();
		}
	}
}