using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level {
	public class Tileset {
		public TextureRegion[] WallA = new TextureRegion[12];
		public TextureRegion[] WallB = new TextureRegion[12];
		public TextureRegion WallTopA;
		public TextureRegion WallTopB;
		public TextureRegion WallCrackA;
		public TextureRegion WallCrackB;
		public TextureRegion[] WallTopsA = new TextureRegion[36];
		public TextureRegion[] WallTopsB = new TextureRegion[36];
		public TextureRegion[] WallSidesA = new TextureRegion[3];
		public TextureRegion[] WallSidesB = new TextureRegion[3];
		
		public TextureRegion[] FloorA = new TextureRegion[16];
		public TextureRegion[] FloorB = new TextureRegion[16];
		public TextureRegion[] FloorC = new TextureRegion[16];
		public TextureRegion[] FloorD = new TextureRegion[16];
		
		public TextureRegion[][] Tiles = new TextureRegion[(int) Tile.Total][];

		public static int[] wallMap = { -1, -1, -1, 9, -1, -1, 0, 5, -1, 11, -1, 10, 2, 6, 1, -1 };
		public static int[] wallMapExtra = { -1, 7, 3, -1, 4, -1, -1, -1, 8, -1, -1, -1, -1, -1, -1, -1 };

		public void Load(string id) {
			var anim = Animations.Get(id);
			
			for (int w = 0; w < 2; w++) {
				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 12; j++) {
						var n = j > 5 ? j + 1 : j;

						if (w == 0) {
							WallTopsA[i * 12 + j] = new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8 + i * 40, n / 5 * 8, 8, 8));
						} else {
							WallTopsB[i * 12 + j] = new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8 + i * 40, 40 + n / 5 * 8, 8, 8));
						}
					}
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

			Tiles[(int) Tile.WallA] = new[] {
				WallTopA = new TextureRegion(anim.Texture, new Rectangle(160, 8, 16, 16))
			};
			
			Tiles[(int) Tile.WallB] = new[] {
				WallTopB = new TextureRegion(anim.Texture, new Rectangle(160, 48, 16, 16))
			};
			
			Tiles[(int) Tile.Crack] = new[] {
				WallCrackA = new TextureRegion(anim.Texture, new Rectangle(176, 8, 16, 16))
			};
			
			WallCrackB = new TextureRegion(anim.Texture, new Rectangle(176, 48, 16, 16));
			
			for (int f = 0; f < 4; f++) {
				for (int i = 0; i < 16; i++) {
					if (f == 0) {
						FloorA[i] = new TextureRegion(anim.Texture, new Rectangle(i % 4 * 16, 80 + i / 4 * 16, 16, 16));
					} else if (f == 1) {
						FloorB[i] = new TextureRegion(anim.Texture, new Rectangle(64 + i % 4 * 16, 80 + i / 4 * 16, 16, 16));
					} else if (f == 2) {
						FloorC[i] = new TextureRegion(anim.Texture, new Rectangle(i % 4 * 16, 144 + i / 4 * 16, 16, 16));
					} else if (f == 3) {
						FloorD[i] = new TextureRegion(anim.Texture, new Rectangle(64 + i % 4 * 16, 144 + i / 4 * 16, 16, 16));
					}
				}

				if (f == 0) {
					Tiles[(int) Tile.FloorA] = FloorA;
				} else if (f == 1) {
					Tiles[(int) Tile.FloorB] = FloorB;
				} else if (f == 2) {
					Tiles[(int) Tile.FloorC] = FloorC;
				} else if (f == 3) {
					Tiles[(int) Tile.FloorD] = FloorD;
				}
			}
		}		
	}
}