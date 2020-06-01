using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;

namespace Desktop.integration.twitch.happening {
	public class RandomItemHappening : Happening {
		private ItemPool pool;

		public RandomItemHappening(ItemPool pl) {
			pool = pl;
		}
		
		public override void Happen(Player player) {
			player.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd(Items.Generate(pool), player.Area));
		}
	}
}