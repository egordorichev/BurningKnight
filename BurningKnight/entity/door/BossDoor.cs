using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class BossDoor : CustomDoor {
		public override void AddComponents() {
			OpenByDefault = false;
			base.AddComponents();
		}

		protected override void SetSize() {
			Width = 24;
			Height = 30;
		}

		protected override Vector2 GetLockOffset() {
			return new Vector2(0, 9);
		}
		
		public override Vector2 GetOffset() {
			return new Vector2(0, 0);
		}

		protected override string GetBar() {
			return "boss_door";
		}

		protected override string GetAnimation() {
			return "boss_door";
		}
		
		protected override Lock CreateLock() {
			return new BossLock();
		}
	}
}