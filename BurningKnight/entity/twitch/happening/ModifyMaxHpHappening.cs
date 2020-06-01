using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class ModifyMaxHpHappening : Happening {
		private int amount;

		public ModifyMaxHpHappening(int a) {
			amount = a;
		}
		
		public override void Happen(Player player) {
			var h = player.GetComponent<HealthComponent>();

			if (amount < 0 && h.MaxHealth <= 1) {
				return;
			}

			h.MaxHealth += amount;
		}
	}
}