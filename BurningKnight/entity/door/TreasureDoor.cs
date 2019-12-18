namespace BurningKnight.entity.door {
	public class TreasureDoor : CustomDoor {
		public override void AddComponents() {
			Width = 24;
			Height = 26;
			
			base.AddComponents();
		}

		protected override string GetBar() {
			return "treasure_door";
		}

		protected override string GetAnimation() {
			return "treasure_door";
		}
	}
}