using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class ChallengeDoor : CustomDoor {
		protected override void SetSize() {
			Width = 40;
			Height = 33;
		}

		protected override Vector2 GetLockOffset() {
			return new Vector2(0, 9);
		}
		
		public override Vector2 GetOffset() {
			return new Vector2(0, 0);
		}

		protected override Lock CreateLock() {
			return new IronLock();
		}

		protected override string GetBar() {
			return "challenge_door";
		}

		protected override string GetAnimation() {
			return "challenge_door";
		}
	}
}