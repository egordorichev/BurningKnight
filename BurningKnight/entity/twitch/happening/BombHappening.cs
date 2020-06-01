using BurningKnight.entity.bomb;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using Lens.util.timer;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.twitch.happening {
	public class BombHappening : Happening {
		public override void Happen(Player player) {
			var rm = player.GetComponent<RoomComponent>().Room;
			
			for (var i = 0; i < 12; i++) {
				Timer.Add(() => {
					var bomb = new Bomb(player, 3f);
					player.Area.Add(bomb);
					bomb.Center = rm.GetRandomFreeTile() * 16 + new Vector2(8);
				}, i * 0.1f);
			}
		}
	}
}