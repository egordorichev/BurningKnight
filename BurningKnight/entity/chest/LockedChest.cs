using BurningKnight.entity.component;
using BurningKnight.entity.door;
using BurningKnight.entity.events;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.chest {
	public class LockedChest : Chest {
		protected override bool CanInteract(Entity e) {
			return base.CanInteract(e) && !GetComponent<LockComponent>().Lock.IsLocked;
		}

		public override void PostInit() {
			base.PostInit();

			if (!IsOpen) {
				AddComponent(new LockComponent(this, new GoldLock(), Vector2.Zero, false) {
					OnOpen = Open
				});
			}
		}

		protected override Entity AlterInteraction() {
			var l = GetComponent<LockComponent>().Lock;
			return l.IsLocked ? l : base.AlterInteraction();
		}
	}
}