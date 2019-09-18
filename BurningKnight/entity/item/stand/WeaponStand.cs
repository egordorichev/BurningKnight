using BurningKnight.assets.items;

namespace BurningKnight.entity.item.stand {
	public class WeaponStand : EmeraldStand {
		protected override bool ApproveItem(ItemData item) {
			return item.Type == ItemType.Weapon;
		}
	}
}