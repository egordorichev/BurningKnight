using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;

namespace Desktop.integration.twitch.happening {
	public class RandomTypedItemHappening : Happening {
		private ItemType pool;

		public RandomTypedItemHappening(ItemType pl) {
			pool = pl;
		}
		
		public override void Happen(Player player) {
			player.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd(Items.Generate(pool), player.Area));
		}
	}
}