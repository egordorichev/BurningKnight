using System;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.level.rooms.treasure {
	public class HoleTreasureRoom : TreasureRoom {
		public override void Paint(Level level) {
			var m = Random.Int(3, 5);
			Painter.Fill(level, this, m, Tile.Chasm);

			m = Math.Max(2, m - (Random.Chance() ? 1 : 2));
			
			PlaceStand(level, new Vector2(Left + m, Top + m) * 16);
			PlaceStand(level, new Vector2(Right - m, Top + m) * 16);
			PlaceStand(level, new Vector2(Left + m, Bottom - m) * 16);
			PlaceStand(level, new Vector2(Right - m, Bottom - m) * 16);
		}
	}
}