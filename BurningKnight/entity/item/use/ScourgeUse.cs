using BurningKnight.state;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ScourgeUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			Run.AddScourge();
		}
	}
}