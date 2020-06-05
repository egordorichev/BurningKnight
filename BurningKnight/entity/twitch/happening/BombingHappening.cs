using BurningKnight.entity.bomb;
using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.twitch.happening {
	public class BombingHappening : Happening {
		private Player on;
		
		public override void Happen(Player player) {
			
		}

		private float delay;

		public override void Update(float dt) {
			base.Update(dt);

			if (on == null || on.Done) {
				on = null;
				return;
			}

			delay -= dt;

			if (delay <= 0) {
				delay = 3;
				var bomb = new Bomb(on, 2);
				on.Area.Add(bomb);
				bomb.Center = on.Center;
			}
		}
	}
}