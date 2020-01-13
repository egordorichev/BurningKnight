using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ExplodeUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			ExplosionMaker.Make(entity);
		}
	}
}