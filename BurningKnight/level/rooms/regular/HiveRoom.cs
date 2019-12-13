using BurningKnight.level.tile;

namespace BurningKnight.level.rooms.regular {
	public class HiveRoom : JungleRoom {
		public HiveRoom() {
			Floor = Tile.FloorD;
			Floor2 = Tile.FloorD;
			Wall = Tile.WallB;
		}
	}
}