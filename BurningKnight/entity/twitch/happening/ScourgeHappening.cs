using BurningKnight.entity.creature.player;
using BurningKnight.state;

namespace BurningKnight.entity.twitch.happening {
	public class ScourgeHappening : Happening {
		private int amount;

		public ScourgeHappening(int a) {
			amount = a;
		}
		
		public override void Happen(Player player) {
			if (amount < 0) {
				for (var i = 0; i < -amount; i++) {
					Run.RemoveScourge();
				}
			} else {
				for (var i = 0; i < amount; i++) {
					Run.AddScourge();
				}
			}
		}
	}
}