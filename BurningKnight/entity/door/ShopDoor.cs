namespace BurningKnight.entity.door {
	public class ShopDoor : LockableDoor {
		protected override Lock CreateLock() {
			return new ShopLock();
		}
	}
}