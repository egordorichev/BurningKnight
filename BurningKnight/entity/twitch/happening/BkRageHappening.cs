using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class BkRageHappening : Happening {
		public override void Happen(Player player) {
			var a = player.Area.Tagged[Tags.BurningKnight];

			if (a.Count == 0) {
				return;
			}
			
			var bk = (creature.bk.BurningKnight) a[0];
			bk.ForcedRage = true;
			bk.CheckForScourgeRage();
		}
	}
}