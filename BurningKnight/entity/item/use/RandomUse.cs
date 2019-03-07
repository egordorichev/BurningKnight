using Lens.entity;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class RandomUse : ItemUse {
		public ItemUse[] Uses;

		public RandomUse(params ItemUse[] uses) {
			Uses = uses;
		}
		
		public void Use(Entity entity, Item item) {
			Uses[Random.Int(Uses.Length)].Use(entity, item);
		}
	}
}