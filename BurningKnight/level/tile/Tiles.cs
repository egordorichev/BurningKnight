using Lens.util.math;

namespace BurningKnight.level.tile {
	public static class Tiles {
		private static Tile lastFloor;
		private static Tile lastWall;
		
		public static Tile Pick(params Tile[] tiles) {
			return tiles[Random.Int(tiles.Length)];
		}

		public static Tile RandomFloor() {
			return lastFloor = Pick(Tile.FloorA, Tile.FloorB, Tile.FloorC);
		}

		public static Tile RandomWall() {
			return Tile.WallA;//lastWall = (Random.Chance() ? Tile.WallA : Tile.WallB);
		}
		
		public static Tile RandomFillWall() {
			return lastWall = Pick(Tile.Planks, Tile.WallA, Tile.WallB);
		}
		
		public static Tile RandomNewWall() {
			switch (lastWall) {
				case Tile.WallA: return Random.Chance(60) ? Tile.WallB : Tile.Planks;
				case Tile.WallB: return Random.Chance(60) ? Tile.WallA : Tile.Planks;
				case Tile.Planks: default: return Random.Chance() ? Tile.WallB : Tile.WallA;
			}
		}

		public static Tile RandomNewFloor() {
			var tiles = new Tile[2];

			switch (lastFloor) {
				case Tile.FloorA: {
					tiles[0] = Tile.FloorB;
					tiles[1] = Tile.FloorC;

					break;
				}

				case Tile.FloorB: {
					tiles[0] = Tile.FloorA;
					tiles[1] = Tile.FloorC;
					
					break;
				}

				default: {
					tiles[0] = Tile.FloorA;
					tiles[1] = Tile.FloorB;
					
					break;
				}
			}

			return tiles[Random.Int(2)];
		}
	}
}