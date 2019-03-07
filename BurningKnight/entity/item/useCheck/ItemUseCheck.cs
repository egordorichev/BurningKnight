using Lens.entity;

namespace BurningKnight.entity.item.useCheck {
	public class ItemUseCheck {
		public bool CanUse(Entity entity, Item item) {
			return item.Delay < 0.05f;
		}
	}
}