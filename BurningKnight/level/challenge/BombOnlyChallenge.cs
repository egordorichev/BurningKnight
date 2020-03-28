using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;

namespace BurningKnight.level.challenge {
	public class BombOnlyChallenge : Challenge {
		public override void Apply(Player player) {
			var inventory = player.GetComponent<InventoryComponent>();
			var area = player.Area;
			
			// Fixme: this is super weak
			inventory.Pickup(Items.CreateAndAdd("bk:blindfold", area), false);
			inventory.Pickup(Items.CreateAndAdd("bk:tnt", area), false);
		}
	}
}