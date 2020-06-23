using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class MakeItemsAttactUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			Item.Attact = true;
		}
	}
}