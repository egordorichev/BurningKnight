using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class TunnelRoom : ConnectionRoom {
		public override void Paint(Level level) {
			var fl = Rnd.Chance() ? Tiles.RandomFloorOrSpike() : Tiles.RandomSolid();
			var w = GenerateSpot();
			
			if (Rnd.Chance()) {
				PaintTunnel(level, fl, w, true);
			}

			if (fl == Tile.Lava) {
				PaintTunnel(level, Tiles.RandomFloorOrSpike(), w);
			}

			if (Rnd.Chance()) {
				PaintTunnel(level, Tiles.RandomFloorOrSpike(), w, true);
			}

			if (Rnd.Chance()) {
				PaintTunnel(level, fl.Matches(Tile.Dirt, Tile.Lava) ? Rnd.Chance() ? 
					Tile.Water : Tile.Dirt : Tiles.RandomFloorOrSpike(), GenerateSpot());
			}
			
			PaintTunnel(level, fl.Matches(Tile.Dirt, Tile.Lava) ? Rnd.Chance() ? 
				Tile.Water : Tile.Dirt : Tiles.RandomFloorOrSpike(), w);
		}
	}
}