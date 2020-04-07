using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class SuperHotUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			if (entity is Player p) {
				p.SuperHot = true;
			}
		}
	}
}