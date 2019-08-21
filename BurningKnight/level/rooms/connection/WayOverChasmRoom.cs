using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class WayOverChasmRoom : ConnectionRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tiles.Pick(Tile.Chasm, Tiles.RandomFloor()));
			
			if (Random.Chance()) {
				PaintTunnel(level, Tiles.Pick(Tile.Chasm, Tile.WallA, Tile.WallB, Tile.Chasm), new Rect(GetCenter()), true);
			}
			
			PaintTunnel(level, Tiles.RandomNewFloor(), Random.Chance() ? GetConnectionSpace() : new Rect(GetCenter()));
			CoverInSpikes(level);
		}

		public override int GetMaxWidth() {
			return 14;
		}

		public override int GetMaxHeight() {
			return 14;
		}
	}
}