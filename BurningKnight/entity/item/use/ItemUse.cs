using Lens.entity;
using Lens.lightJson;
using Lens.util.file;

namespace BurningKnight.entity.item.use {
	public abstract class ItemUse {
		public abstract void Use(Entity entity, Item item);

		public virtual void Setup(JsonValue settings) {
			
		}
	}
}