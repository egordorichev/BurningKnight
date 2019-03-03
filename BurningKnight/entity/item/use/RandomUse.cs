using BurningKnight.entity.creature.player;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class RandomUse : ItemUse {
		public ItemUse[] Uses;

		public RandomUse(params ItemUse[] uses) {
			Uses = uses;
		}
		
		public void Use(Player player, Item item) {
			Uses[Random.Int(Uses.Length)].Use(player, item);
		}
	}
}