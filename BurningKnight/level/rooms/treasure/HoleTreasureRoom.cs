using System;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class HoleTreasureRoom : TreasureRoom {
		public override void Paint(Level level) {
			var m = Rnd.Int(3, 5);
			Painter.Fill(level, this, m, Tiles.RandomSolid());

			m = Math.Max(2, m - (Rnd.Chance() ? 1 : 2));
			
			PlaceStand(level, new Dot(Left + m, Top + m));
			PlaceStand(level, new Dot(Right - m, Top + m));
			PlaceStand(level, new Dot(Left + m, Bottom - m));
			PlaceStand(level, new Dot(Right - m, Bottom - m));
			
			SetupStands(level);
		}
	}
}