using BurningKnight.assets.lighting;
using BurningKnight.entity.creature.player;
using BurningKnight.state;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.twitch.happening {
	public class DarknessHappening : Happening {
		public override void Happen(Player player) {
			Run.Level.Dark = true;
			Lights.ClearColor = new Color(0f, 0f, 0f, 1f);
		}
	}
}