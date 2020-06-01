using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class MakeItemsDamageUse : Happening {
		public override void Happen(Player player) {
			player.ItemDamage = true;
		}
	}
}