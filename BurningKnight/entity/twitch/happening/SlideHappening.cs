using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class SlideHappening : Happening {
		public override void Happen(Player player) {
			player.Sliding = true;
		}

		public override void End(Player player) {
			player.Sliding = false;
		}

		public override float GetVoteDelay() {
			return 60;
		}
	}
}