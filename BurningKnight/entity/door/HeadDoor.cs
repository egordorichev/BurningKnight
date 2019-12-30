using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class HeadDoor : CustomDoor {
		protected override void SetSize() {
			Width = 30;
			Height = 25;
		}
		
		protected override Rectangle GetHitbox() {
			return new Rectangle(0, 5 + 8, (int) Width, 7);
		}
		
		protected override Vector2 GetLockOffset() {
			return new Vector2(0, 7);
		}
		
		public override Vector2 GetOffset() {
			return new Vector2(0, 0);
		}

		protected override Lock CreateLock() {
			return new IronLock();
		}

		protected override string GetBar() {
			return "head_door";
		}

		protected override string GetAnimation() {
			return "head_door";
		}
	}
}