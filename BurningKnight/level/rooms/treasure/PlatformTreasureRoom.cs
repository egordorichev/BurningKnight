using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.treasure {
	public class PlatformTreasureRoom : TreasureRoom {
		public override void Paint(Level level) {
			base.Paint(level);
			
			Painter.Fill(level, this, 1, Tile.Chasm);

			var f = Tiles.RandomFloor();
			Painter.Fill(level, this, Random.Int(2, 4), f);
			
			PaintTunnel(level, Random.Chance(30) ? f : Tiles.RandomFloor(), GetCenterRect());
		}
	}
}