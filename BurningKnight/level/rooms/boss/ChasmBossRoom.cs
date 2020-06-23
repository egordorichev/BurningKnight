using BurningKnight.level.tile;
using Lens;
using Lens.util.math;

namespace BurningKnight.level.rooms.boss {
	public class ChasmBossRoom : BossRoom {
		protected override void PaintRoom(Level level) {
			var m = 1;
			
			Painter.Fill(level, this, Tile.WallA);
			Painter.Fill(level, this, m, Tile.Chasm);

			m += Rnd.Int(0, 2);
			
			if (Rnd.Chance()) {
				Painter.FillEllipse(level, this, m, Tiles.RandomFloor());
			} else {
				Painter.Fill(level, this, m, Tiles.RandomFloor());
			}

			m++;
			
			if (Rnd.Chance()) {
				Painter.FillEllipse(level, this, m, Tiles.RandomNewFloor());
			} else {
				Painter.Fill(level, this, m, Tiles.RandomNewFloor());
			}

			if (Rnd.Chance()) {
				PaintTunnel(level, Tiles.RandomFloor(), GetCenterRect(), true);
			}
			
			PaintTunnel(level, Tiles.RandomFloor(), GetCenterRect());
		}
		
		public override int GetMinWidth() {
			return Display.Width / 16 - 5;
		}

		public override int GetMinHeight() {
			return Display.Width / 16 - 3;
		}

		public override int GetMaxWidth() {
			return Display.Width / 16 - 5;
		}

		public override int GetMaxHeight() {
			return Display.Width / 16 - 2;
		}
	}
}