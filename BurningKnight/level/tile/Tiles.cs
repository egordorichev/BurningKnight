using BurningKnight.save;
using BurningKnight.state;
using Lens.util.math;

namespace BurningKnight.level.tile {
	public static class Tiles {
		private static Tile lastFloor;
		private static Tile lastWall;
		
		public static Tile Pick(params Tile[] tiles) {
			return tiles[Rnd.Int(tiles.Length)];
		}

		public static Tile RandomFloor(bool gold = false) {
			if (gold) {
				return lastFloor = Pick(Tile.FloorA, Tile.FloorB, Tile.FloorC, Tile.FloorD);
			}
			
			return lastFloor = Pick(Tile.FloorA, Tile.FloorB, Tile.FloorC);
		}
		
		public static Tile RandomFloorOrSpike() {
			return lastFloor = (LevelSave.BiomeGenerated?.HasSpikes() ?? true) && Rnd.Chance(20) ? Tile.SensingSpikeTmp : Pick(Tile.FloorA, Tile.FloorB, Tile.FloorC);
		}
		
		public static Tile RandomWall() {
			return lastWall = Pick(Tile.WallA, Tile.Rock, Tile.MetalBlock);
		}
		
		public static Tile RandomSolid() {
			return lastWall = Pick(Tile.WallA, Tile.Rock, Tile.MetalBlock, Tile.Planks, Tile.Lava, Tile.Planks, (LevelSave.BiomeGenerated?.HasSpikes() ?? true) ? Tile.SensingSpikeTmp : Tile.WallA);
		}
		
		public static Tile RandomFillWall() {
			return lastWall = Pick(Tile.WallA, Tile.Rock, Tile.MetalBlock, Tile.Planks);
		}
		
		public static Tile RandomNewWall() {
			switch (lastWall) {
				case Tile.WallA: return Pick(Tile.Planks, Tile.MetalBlock, Tile.Rock);
				case Tile.Planks: return Pick(Tile.WallA, Tile.MetalBlock, Tile.Rock);
				case Tile.MetalBlock: return Pick(Tile.WallA, Tile.Planks, Tile.Rock);
				case Tile.Rock: default: return Pick(Tile.WallA, Tile.MetalBlock, Tile.Planks);
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

			return tiles[Rnd.Int(2)];
		}
	}
}