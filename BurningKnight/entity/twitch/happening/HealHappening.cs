using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class HealHappening : Happening {
		private int amount;

		public HealHappening(int a) {
			amount = a;
		}
		
		public override void Happen(Player player) {
			player.GetComponent<HealthComponent>().ModifyHealth(amount, player);
		}
	}
}