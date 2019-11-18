using BurningKnight.entity.item.use.parent;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class RandomUse : DoUsesUse {
		protected override void DoAction(Entity entity, Item item, ItemUse use) {
			
		}

		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			Uses[Rnd.Int(Uses.Length)].Use(entity, item);
		}
	}
}