using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public abstract class ItemUse {
		public abstract void Use(Entity entity, Item item);

		public virtual void Setup(JsonValue settings) {
			
		}
	}
}