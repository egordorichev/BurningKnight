using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.rooms.boss {
	public class ChasmBossRoom : BossRoom {
		private void PaintRoad(Level level) {
			if (Random.Chance()) {
				PaintTunnel(level, Tiles.RandomFloor(), null, true);
			}
			
			PaintTunnel(level, Tiles.RandomNewFloor(), null, true);
		}
		
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.WallA);
			Painter.Fill(level, GetCenterRect(), -2, Tile.FloorA);
			PaintTunnel(level, Tile.FloorB);

			/*var before = Random.Chance();

			if (before) {
				PaintRoad(level);
			}
			
			if (Random.Chance()) {
				Painter.FillEllipse(level, this, 3, Tiles.RandomFloor());
			} else {
				Painter.Fill(level, this, 3, Tiles.RandomFloor());
			}

			if (!before) {
				PaintRoad(level);
			}*/

			Place(level);
		}

		public override Rect GetConnectionSpace() {
			return GetCenterRect();
		}
	}
}