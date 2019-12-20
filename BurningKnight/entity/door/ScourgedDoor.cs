using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class ScourgedDoor : CustomDoor {
		protected override Rectangle GetHitbox() {
			return new Rectangle(0, 5 + 4, (int) Width, 7);
		}
		
		protected override Vector2 GetLockOffset() {
			return new Vector2(0, 3);
		}

		protected override void SetSize() {
			Width = 24;
			Height = 23;
		}
		
		public override Vector2 GetOffset() {
			return new Vector2(0, 0);
		}

		protected override Lock CreateLock() {
			return new IronLock();
		}

		protected override string GetBar() {
			return "scourged_door";
		}

		protected override string GetAnimation() {
			return "scourged_door";
		}
	}
}