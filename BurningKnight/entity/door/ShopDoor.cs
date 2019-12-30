using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class ShopDoor : CustomDoor {
		protected override void SetSize() {
			Width = Vertical ? 14 : 24;
			Height = Vertical ? 17 : 31;

			OpenByDefault = !Vertical;
		}

		public override Vector2 GetOffset() {
			return new Vector2(0, Vertical ? -6 : 0);
		}

		protected override Vector2 GetLockOffset() {
			return Vertical ? new Vector2(0, -1) : new Vector2(0, 9);
		}
		
		protected override Rectangle GetHitbox() {
			return Vertical ? new Rectangle(0, 0, (int) Width, (int) Height + 5) 
				: new Rectangle(0, 14, (int) Width, 16);
		}

		protected override Lock CreateLock() {
			return new GoldLock();
		}

		protected override string GetBar() {
			return Vertical ? null : "shop_door";
		}

		protected override string GetAnimation() {
			return Vertical ? "vertical_shop_door" : "shop_door";
		}

		protected override string GetPad() {
			return null;
		}
	}
}