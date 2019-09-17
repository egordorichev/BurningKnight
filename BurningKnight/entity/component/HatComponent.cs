using BurningKnight.entity.item;

namespace BurningKnight.entity.component {
	public class HatComponent : ItemComponent {
		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Hat;
		}
	}
}