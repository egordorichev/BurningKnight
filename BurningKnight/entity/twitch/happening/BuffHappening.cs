using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class BuffHappening : Happening {
		private string buff;
		private float duration;

		public BuffHappening(string b, float d = 10f) {
			buff = b;
			duration = d;
		}
		
		public override void Happen(Player player) {
			player.GetComponent<BuffsComponent>().Add(buff).TimeLeft = duration;
		}
	}
}