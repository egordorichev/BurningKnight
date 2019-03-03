using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.item.useCheck {
	public class ItemUseCheck {
		public bool CanUse(Player player, Item item) {
			return item.Delay == 0;
		}
	}
}