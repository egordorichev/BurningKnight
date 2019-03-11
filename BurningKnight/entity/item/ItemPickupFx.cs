using Lens.entity;

namespace BurningKnight.entity.item {
	public class ItemPickupFx : Entity {
		private string text;

		public ItemPickupFx(Item item) {
			text = item.Name;
		}
	}
}