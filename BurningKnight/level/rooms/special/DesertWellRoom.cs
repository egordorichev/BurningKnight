using System;
using BurningKnight.level.rooms.regular;
using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.special {
	public class DesertWellRoom : RegularRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, Tile.Grass);

			if (GetWidth() * GetHeight() < 25) {
				Painter.Set(level, GetCenter(), Tile.Water);
				return;
			}

			var smallest = Math.Min(GetWidth(), GetHeight());
			var floorW = (int) Math.Sqrt(smallest);
			var edgeFloorChance = (float) Math.Sqrt(smallest) % 1;

			edgeFloorChance = (edgeFloorChance + (floorW - 1) * 0.5f) / (float) floorW;

			for (var y = Top + 2; y < Bottom - 2; y++) {
				for (var x = Left + 2; x < Right - 2; x++) {
					var v = Math.Min(y - Top, Bottom - y);
					var h = Math.Min(x - Left, Right - x);

					if (Math.Min(v, h) > floorW || (Math.Min(v, h) == floorW && Rnd.Float() > edgeFloorChance)) {
						Painter.Set(level, x, y, Tile.Water);
					}
				}
			}
			
			Painter.PlacePlants(level, this);
			Painter.PlaceTrees(level, this);
		}
	}
}