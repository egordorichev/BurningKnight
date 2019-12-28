using BurningKnight.state;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.door {
	public class TreasureDoor : CustomDoor {
		protected override void SetSize() {
			Width = Vertical ? 10 : 24;
			Height = Vertical ? 22 : 26;
			OpenByDefault = !Vertical;
		}

		public override Vector2 GetOffset() {
			return new Vector2(0, Vertical ? -5 : 0);
		}

		protected override Vector2 GetLockOffset() {
			return Vertical ? new Vector2(0, 0) : new Vector2(0, 6);
		}
		
		protected override Rectangle GetHitbox() {
			return Vertical ? new Rectangle(0, 0, (int) Width, (int) Height + 3) 
				: new Rectangle(0, 12, (int) Width, 7);
		}

		// finish roger (room full of tnt boxes, animation, pool, unique items, etc)
		protected override Lock CreateLock() {
			return Run.Depth == 1 ? (Lock) new IronLock() : (Lock) new GoldLock();
		}

		protected override string GetBar() {
			return Vertical ? "vertical_treasure_door" : "treasure_door";
		}

		protected override string GetAnimation() {
			return Vertical ? "vertical_treasure_door" : "treasure_door";
		}

		protected override string GetPad() {
			return Vertical ? "vertical_treasure_door_pad" : null;
		}
	}
}