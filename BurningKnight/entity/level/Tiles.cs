using Lens.util.math;

namespace BurningKnight.entity.level {
	public static class Tiles {
		private static Tile lastFloor;
		
		public static Tile Pick(params Tile[] tiles) {
			return tiles[Random.Int(tiles.Length)];
		}

		public static Tile RandomFloor() {
			return lastFloor = Pick(Tile.FloorA, Tile.FloorB, Tile.FloorC);
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