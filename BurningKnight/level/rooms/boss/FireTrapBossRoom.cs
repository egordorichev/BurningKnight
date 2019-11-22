using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.rooms.boss {
	public class FireTrapBossRoom : BossRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.FireTrapTmp);

			var w = GetWidth();
			var a = Tiles.RandomFloor();
			var b = Rnd.Chance() ? a : Tiles.RandomNewFloor(); 
			var collumnSize = (int) (w / 6f);
			
			Painter.Fill(level, Left + w / 4 - collumnSize, Top + w / 4 - collumnSize, collumnSize * 2, collumnSize * 2, a);
			Painter.Fill(level, Right - w / 4 - collumnSize, Top + w / 4 - collumnSize, collumnSize * 2, collumnSize * 2, b);
			Painter.Fill(level, Left + w / 4 - collumnSize, Bottom - w / 4 - collumnSize, collumnSize * 2, collumnSize * 2, b);
			Painter.Fill(level, Right - w / 4 - collumnSize, Bottom - w / 4 - collumnSize, collumnSize * 2, collumnSize * 2, a);

			var c = GetCenter();
			
			Painter.Fill(level, c.X - 1, c.Y - 1, 3, 3, Tiles.RandomNewFloor());
		}
	}
}