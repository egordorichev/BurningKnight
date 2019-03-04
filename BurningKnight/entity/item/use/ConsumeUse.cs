using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.item.use {
	public class ConsumeUse : ItemUse {
		public virtual void Use(Player player, Item item) {
			item.Count--;
		}
	}
}