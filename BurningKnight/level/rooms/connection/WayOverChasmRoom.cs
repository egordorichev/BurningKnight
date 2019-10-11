using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class WayOverChasmRoom : ConnectionRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Random.Chance(70) ? Tile.Chasm : Tiles.RandomFloor());

			var r = GenerateSpot();
			
			if (Random.Chance()) {
				PaintTunnel(level, Tiles.Pick(Tile.Chasm, Tile.WallA, Tiles.RandomFloorOrSpike()), Random.Chance() ? r : GenerateSpot(), true);
			}
		
			PaintTunnel(level, Tiles.RandomFloorOrSpike(), r);
		}

		public override int GetMaxWidth() {
			return 14;
		}

		public override int GetMaxHeight() {
			return 14;
		}
	}
}