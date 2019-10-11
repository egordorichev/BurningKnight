using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class HoleConnectionRoom : ConnectionRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, Left + 2, Top + 2, GetWidth() - 4, GetHeight() - 4, Random.Chance() ? Tile.WallA : Tile.Chasm);
		}
		
		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}
	}
}