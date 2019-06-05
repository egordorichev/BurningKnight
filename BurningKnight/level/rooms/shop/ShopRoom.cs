using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;

namespace BurningKnight.level.rooms.shop {
	public class ShopRoom : LockedRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 2, Tile.FloorD);
		}
	}
}