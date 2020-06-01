using System;
using BurningKnight.entity.creature.player;
using BurningKnight.level.entities.chest;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.twitch.happening {
	public class ChestHappening : Happening {
		public override void Happen(Player player) {
			try {
				ChestRegistry.PlaceRandom(player.TopCenter + new Vector2(0, -2), player.Area);
			} catch (Exception ex) {
				Log.Error(ex);
			}
		}
	}
}