using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class GiveShieldHappening : Happening {
		public override void Happen(Player player) {
			player.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd("bk:shield", player.Area));
		}
	}
}