using BurningKnight.assets.items;
using BurningKnight.save;
using BurningKnight.util;
using Lens;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class LampUnlockStand : ItemStand {
		public override void PostInit() {
			base.PostInit();

			CheckHidden();
		}

		protected override string GetSprite() {
			return "single_stand";
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);
			Items.Unlock(item.Id);
			CheckHidden();
		}

		public void CheckHidden() {
			if (!Engine.EditingLevel && (Item == null || GlobalSave.IsTrue(Item.Id))) {
				Hidden = true;
				AnimationUtil.Poof(Center);
			} else {
				Hidden = false;
			}
		}
	}
}