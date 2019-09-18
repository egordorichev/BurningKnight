using BurningKnight.assets.items;
using BurningKnight.save;

namespace BurningKnight.entity.item.stand {
	public class HatStand : EmeraldStand {
		public HatStand() {
			ShowUnlocked = true;
		}
		
		protected override bool ApproveItem(ItemData item) {
			return item.Type == ItemType.Hat && GlobalSave.GetString("hat") != item.Id;
		}
	}
}