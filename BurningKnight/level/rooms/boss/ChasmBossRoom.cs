using BurningKnight.level.tile;

namespace BurningKnight.level.rooms.boss {
	public class ChasmBossRoom : BossRoom {
		protected override void PaintRoom(Level level) {
			Painter.Fill(level, this, Tile.WallA);
			Painter.Fill(level, this, 1, Tile.Chasm);
			Painter.Fill(level, this, 3, Tiles.RandomFloor());
			
			PaintTunnel(level, Tiles.RandomFloor());
		}
	}
}