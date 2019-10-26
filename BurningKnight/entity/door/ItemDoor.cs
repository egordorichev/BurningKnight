namespace BurningKnight.entity.door {
	public class ItemDoor : LockableDoor {
		protected override Lock CreateLock() {
			return new ItemLock();
		}
	}
}