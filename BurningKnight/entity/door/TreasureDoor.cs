using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class TreasureDoor : CustomDoor {
		public override void AddComponents() {
			Width = 24;
			Height = 26;
			
			base.AddComponents();
		}
		
		protected override Vector2 GetLockOffset() {
			return FacingSide ? new Vector2(0, -4) : new Vector2(0, 6);
		}

		protected override Lock CreateLock() {
			return new GoldLock();
		}

		protected override string GetBar() {
			return "treasure_door";
		}

		protected override string GetAnimation() {
			return "treasure_door";
		}
	}
}