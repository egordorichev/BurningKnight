using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class TunnelRoom : ConnectionRoom {
		public override void Paint(Level level) {
			if (level.GetFilling() == Tile.Chasm || Rnd.Chance()) {
				Painter.Fill(level, this, 1, Tile.Chasm);
			}
		
			PaintTunnel(level, Tiles.RandomFloorOrSpike(),  GenerateSpot());
		}
	}
}