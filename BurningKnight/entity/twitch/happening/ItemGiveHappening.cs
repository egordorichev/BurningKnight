using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class ItemGiveHappening : Happening {
		private Func<string> id;

		public ItemGiveHappening(Func<string> i) {
			id = i;
		}
		
		public override void Happen(Player player) {
			player.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd(id(), player.Area));
		}
	}
}