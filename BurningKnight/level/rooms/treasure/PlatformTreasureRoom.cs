using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class PlatformTreasureRoom : TreasureRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tiles.RandomSolid());

			var f = Tiles.RandomFloor();
			var m = Random.Int(2, 4);
			
			Painter.Fill(level, this, m, f);

			if ((GetWidth() > m * 2 + 1 && GetHeight() > m * 2 + 1)) {
				m++;
			}
			
			PlaceStand(level, new Dot(Left + m, Top + m) * 16);
			PlaceStand(level, new Dot(Right - m, Top + m) * 16);
			PlaceStand(level, new Dot(Left + m, Bottom - m) * 16);
			PlaceStand(level, new Dot(Right - m, Bottom - m) * 16);

			PaintTunnel(level, Random.Chance(30) ? f : Tiles.RandomFloor(), GetCenterRect());
			
			SetupStands(level);
		}

		public override int GetMinWidth() {
			return 10;
		}

		public override int GetMinHeight() {
			return 10;
		}
	}
}