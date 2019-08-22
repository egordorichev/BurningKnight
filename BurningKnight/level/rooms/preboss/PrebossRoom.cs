using BurningKnight.level.rooms.entrance;
using BurningKnight.level.tile;

namespace BurningKnight.level.rooms.preboss {
	public class PrebossRoom : EntranceRoom {
		public PrebossRoom() {
			Exit = true;
		}
		
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tiles.RandomFloor());
			Painter.Fill(level, this, 3, Tile.Chasm);
		}
	}
}