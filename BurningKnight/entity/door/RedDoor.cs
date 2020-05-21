namespace BurningKnight.entity.door {
	public class RedDoor : SpecialDoor {
		protected override Lock CreateLock() {
			return new RedLock();
		}
	}
}