namespace BurningKnight.entity.door {
	public class BossDoor : LockableDoor {
		protected override Lock CreateLock() {
			return new BossLock();
		}
	}
}