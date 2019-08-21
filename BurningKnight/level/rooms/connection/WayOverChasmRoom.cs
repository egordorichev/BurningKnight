using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class WayOverChasmRoom : ConnectionRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Random.Chance(70) ? Tile.Chasm :Tiles.RandomFloor());
			
			if (Random.Chance()) {
				PaintTunnel(level, Tiles.Pick(Tile.Chasm, Tile.WallA, Tile.Chasm), new Rect(GetCenter()), true);
			}
			
			PaintTunnel(level, Tiles.RandomFloorOrSpike(), Random.Chance() ? GetConnectionSpace() : new Rect(GetCenter()));
		}

		public override int GetMaxWidth() {
			return 14;
		}

		public override int GetMaxHeight() {
			return 14;
		}
	}
}