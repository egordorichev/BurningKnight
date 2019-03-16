using BurningKnight.entity.component;
using BurningKnight.entity.door;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.chest {
	public class LockedChest : Chest {
		protected override bool CanInteract() {
			return base.CanInteract() && !GetComponent<LockComponent>().Lock.IsLocked;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new LockComponent(this, new GoldLock(), Vector2.Zero));
		}
	}
}