using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.boss {
	public class ChasmBossRoom : BossRoom {
		protected override void PaintRoom(Level level) {
			var m = 1;
			
			Painter.Fill(level, this, Tile.WallA);
			Painter.Fill(level, this, m, Tile.Chasm);

			m += Random.Int(1, 3);
			
			if (Random.Chance()) {
				Painter.FillEllipse(level, this, m, Tiles.RandomFloor());
			} else {
				Painter.Fill(level, this, m, Tiles.RandomFloor());
			}

			m++;
			
			if (Random.Chance()) {
				Painter.FillEllipse(level, this, m, Tiles.RandomNewFloor());
			} else {
				Painter.Fill(level, this, m, Tiles.RandomNewFloor());
			}

			if (Random.Chance()) {
				PaintTunnel(level, Tiles.RandomFloor(), GetCenterRect(), true);
			}
			
			PaintTunnel(level, Tiles.RandomFloor(), GetCenterRect());
		}
	}
}