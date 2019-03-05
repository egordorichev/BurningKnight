using BurningKnight.entity.component;
using BurningKnight.entity.item;

namespace BurningKnight.entity.creature.player {
	public class WeaponComponent : ItemComponent {
		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Normal;
		}
	}
}