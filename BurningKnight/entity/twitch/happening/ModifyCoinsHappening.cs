using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class ModifyCoinsHappening : Happening {
		private int amount;

		public ModifyCoinsHappening(int a) {
			amount = a;
		}
		
		public override void Happen(Player player) {
			var c = player.GetComponent<ConsumablesComponent>();
			c.Coins += amount;
		}
	}
}