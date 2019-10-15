using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class TunnelRoom : ConnectionRoom {
		public override void Paint(Level level) {
			var fl = Random.Chance() ? Tiles.RandomFloorOrSpike() : Tiles.Pick(Tile.Chasm, Tile.Lava, Tile.WallA, Tile.Planks);
			var w = GenerateSpot();
			
			if (Random.Chance()) {
				PaintTunnel(level, fl, w, true);
			}

			if (fl == Tile.Lava) {
				PaintTunnel(level, Tiles.RandomFloorOrSpike(), w);
			}

			if (Random.Chance()) {
				PaintTunnel(level, Tiles.RandomFloorOrSpike(), w, true);
			}

			if (Random.Chance()) {
				PaintTunnel(level, fl.Matches(Tile.Dirt, Tile.Lava) ? Random.Chance() ? 
					Tile.Water : Tile.Dirt : Tiles.RandomFloorOrSpike(), GenerateSpot());
			}
			
			PaintTunnel(level, fl.Matches(Tile.Dirt, Tile.Lava) ? Random.Chance() ? 
				Tile.Water : Tile.Dirt : Tiles.RandomFloorOrSpike(), w);
		}
	}
}