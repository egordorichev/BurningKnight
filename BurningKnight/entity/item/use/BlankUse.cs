using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class BlankUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			BlankMaker.Make(entity.Center, entity.Area);
		}
	}
}