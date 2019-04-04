using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ConsumeUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			item.Count--;
		}
	}
}