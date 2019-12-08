using System;
using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.tile {
	public class Tileset {
		public TextureRegion[] WallA = new TextureRegion[12];
		public TextureRegion[] WallB = new TextureRegion[12];
		public TextureRegion[] WallAExtensions = new TextureRegion[4];
		public TextureRegion[] WallBExtensions = new TextureRegion[4];
		public TextureRegion WallTopA;
		public TextureRegion WallTopB;
		public TextureRegion WallTopADecor;
		public TextureRegion WallTopBDecor;
		public TextureRegion WallCrackA;
		public TextureRegion WallCrackB;
		public TextureRegion[] WallTopsA = new TextureRegion[36];
		public TextureRegion[] WallTopsB = new TextureRegion[36];
		public TextureRegion[] WallTopsTransition = new TextureRegion[36];
		public TextureRegion[] WallSidesA = new TextureRegion[3];
		public TextureRegion[] WallSidesB = new TextureRegion[3];
		
		public TextureRegion[] FloorA = new TextureRegion[16];
		public TextureRegion[] FloorB = new TextureRegion[16];
		public TextureRegion[] FloorC = new TextureRegion[16];
		public TextureRegion[] FloorD = new TextureRegion[16];
		
		public TextureRegion[] FloorSidesA = new TextureRegion[4];
		public TextureRegion[] FloorSidesB = new TextureRegion[4];
		public TextureRegion[] FloorSidesC = new TextureRegion[4];
		public TextureRegion[] FloorSidesD = new TextureRegion[4];
		
		public TextureRegion[] WallVariants = new TextureRegion[8];
		
		public TextureRegion[] MetalBlock = new TextureRegion[4];
		public TextureRegion[] Rock = new TextureRegion[4];
		public TextureRegion[] TintedRock = new TextureRegion[4];
		public TextureRegion MetalBlockShadow;
		
		public TextureRegion[][] Tiles = new TextureRegion[(int) Tile.Total][];

		public static int[] WallMap = { -1, -1, -1, 9, -1, -1, 0, 5, -1, 11, -1, 10, 2, 6, 1, -1 };
		public static int[] WallMapExtra = { -1, 7, 3, -1, 4, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1 };
		
		public void Load(string id) {
			var anim = Animations.Get(id);
			
			WallVariants = new TextureRegion[8];
			
			for (int i = 0; i < 8; i++) {
				WallVariants[i] = new TextureRegion(anim.Texture, new Rectangle(128 + i % 4 * 16, 160 + (int) Math.Floor(i / 4f) * 16, 16, 16));
			}
			
			for (int w = 0; w < 3; w++) {
				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 12; j++) {
						var n = j > 5 ? j + 1 : j;

						if (w == 0) {
							WallTopsA[i * 12 + j] = new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8 + i * 40, n / 5 * 8, 8, 8));
						} else if (w == 1) {
							WallTopsB[i * 12 + j] = new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8 + i * 40, 40 + n / 5 * 8, 8, 8));
						} else {
							WallTopsTransition[i * 12 + j] = new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8 + i * 40, 240 + n / 5 * 8, 8, 8));
						}
					}
				}
				
				if (w == 0) {
					WallAExtensions[0] = new TextureRegion(anim.Texture, new Rectangle(208, 0, 16, 8));
					WallAExtensions[1] = new TextureRegion(anim.Texture, new Rectangle(224, 8, 8, 16));
					WallAExtensions[2] = new TextureRegion(anim.Texture, new Rectangle(208, 24, 16, 8));
					WallAExtensions[3] = new TextureRegion(anim.Texture, new Rectangle(200, 8, 8, 16));
				} else {
					WallBExtensions[0] = new TextureRegion(anim.Texture, new Rectangle(208, 40, 16, 8));
					WallBExtensions[1] = new TextureRegion(anim.Texture, new Rectangle(224, 48, 8, 16));
					WallBExtensions[2] = new TextureRegion(anim.Texture, new Rectangle(208, 64, 16, 8));
					WallBExtensions[3] = new TextureRegion(anim.Texture, new Rectangle(200, 48, 8, 16));
				}
				
				for (int i = 0; i < 12; i++) {
					if (w == 0) {
						WallA[i] = new TextureRegion(anim.Texture, new Rectangle(i * 16, 24, 16, 16));
					} else {
						WallB[i] = new TextureRegion(anim.Texture, new Rectangle(i * 16, 64, 16, 16));
					}
				}

				for (int i = 0; i < 3; i++) {
					if (w == 0) {
						WallSidesA[i] = new TextureRegion(anim.Texture, new Rectangle(128 + i * 16, 80, 16, 16));
					} else {
						WallSidesB[i] = new TextureRegion(anim.Texture, new Rectangle(128 + i * 16, 96, 16, 16));
					}
				}
			}

			WallTopADecor = new TextureRegion(anim.Texture, new Rectangle(128, 0, 16, 16));
			WallTopBDecor = new TextureRegion(anim.Texture, new Rectangle(144, 0, 16, 16));

			Tiles[(int) Tile.PistonDown] = Tiles[(int) Tile.Piston] = Tiles[(int) Tile.WallA] = new[] {
				WallTopA = new TextureRegion(anim.Texture, new Rectangle(160, 8, 16, 16))
			};
			
			Tiles[(int) Tile.WallB] = new[] {
				WallTopB = new TextureRegion(anim.Texture, new Rectangle(160, 48, 16, 16))
			};
			
			Tiles[(int) Tile.Crack] = new[] {
				WallCrackA = new TextureRegion(anim.Texture, new Rectangle(176, 8, 16, 16))
			};
			
			WallCrackB = new TextureRegion(anim.Texture, new Rectangle(176, 48, 16, 16));

			for (int f = 0; f < 3; f++) {
				for (int i = 0; i < 4; i++) {
					if (f == 0) {
						MetalBlock[i] = new TextureRegion(anim.Texture, new Rectangle(128 + i % 2 * 16, 192 + i / 2 * 16, 16, 16));
					} else if (f == 1) {
						Rock[i] = new TextureRegion(anim.Texture, new Rectangle(160 + i % 2 * 16, 192 + i / 2 * 16, 16, 16));
					} else {
						TintedRock[i] = new TextureRegion(anim.Texture, new Rectangle(160 + i % 2 * 16, 224 + i / 2 * 16, 16, 16));
					}
				}
			}

			MetalBlockShadow = new TextureRegion(anim.Texture, new Rectangle(128, 224, 16, 16));

			for (int f = 0; f < 4; f++) {
				for (int i = 0; i < 16; i++) {
					if (f == 0) {
						FloorA[i] = new TextureRegion(anim.Texture, new Rectangle(i % 4 * 16, 80 + i / 4 * 16, 16, 16));
					} else if (f == 1) {
						FloorB[i] = new TextureRegion(anim.Texture, new Rectangle(64 + i % 4 * 16, 80 + i / 4 * 16, 16, 16));
					} else if (f == 2) {
						FloorC[i] = new TextureRegion(anim.Texture, new Rectangle(i % 4 * 16, 160 + i / 4 * 16, 16, 16));
					} else if (f == 3) {
						FloorD[i] = new TextureRegion(anim.Texture, new Rectangle(64 + i % 4 * 16, 160 + i / 4 * 16, 16, 16));
					}
				}

				for (int i = 0; i < 4; i++) {
					if (f == 0) {
						FloorSidesA[i] = new TextureRegion(anim.Texture, new Rectangle(i * 16, 144, 16, 16));
					} else if (f == 1) {
						FloorSidesB[i] = new TextureRegion(anim.Texture, new Rectangle(64 + i * 16, 144, 16, 16));
					} else if (f == 2) {
						FloorSidesC[i] = new TextureRegion(anim.Texture, new Rectangle(i * 16, 224, 16, 16));
					} else if (f == 3) {
						FloorSidesD[i] = new TextureRegion(anim.Texture, new Rectangle(64 + i * 16, 224, 16, 16));
					}
				}
			}
			
			Tiles[(int) Tile.FloorA] = FloorA;
			Tiles[(int) Tile.FloorB] = FloorB;
			Tiles[(int) Tile.FloorC] = FloorC;
			Tiles[(int) Tile.FloorD] = FloorD;
			
			Tiles[(int) Tile.Rock] = Rock;
			Tiles[(int) Tile.TintedRock] = TintedRock;
			Tiles[(int) Tile.MetalBlock] = MetalBlock;
		}
	}
}