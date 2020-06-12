using System;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class HoleTreasureRoom : TreasureRoom {
		public override void PaintInside(Level level) {
			var m = Rnd.Int(4, 6);
			Painter.Fill(level, this, m, Tiles.RandomSolid());

			m = Math.Max(3, m - (Rnd.Chance() ? 1 : 2));

			var v = Rnd.Chance();
			var h = !v || Rnd.Chance();

			if (v) {
				PlaceStand(level, new Dot(Left + m, Top + m));
				PlaceStand(level, new Dot(Right - m, Bottom - m));
			}

			if (h) {
				PlaceStand(level, new Dot(Left + m, Bottom - m));
				PlaceStand(level, new Dot(Right - m, Top + m));
			}

			SetupStands(level);
		}
		
		public override int GetMinWidth() {
			return 10;
		}

		public override int GetMaxWidth() {
			return 14;
		}

		public override int GetMinHeight() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 14;
		}
		
	}
}