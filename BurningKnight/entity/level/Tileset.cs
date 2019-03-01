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
		
		public void Load(string id) {
			var anim = Animations.Get(id);

			for (int w = 0; w < 2; w++) {
				for (int i = 0; i < 12; i++) {
					if (w == 0) {
						WallA[i] = new TextureRegion(anim.Texture, new Rectangle(i * 16, 24, 16, 16));
					} else {
						WallB[i] = new TextureRegion(anim.Texture, new Rectangle(i * 16, 64, 16, 16));
					}
				}

				for (int i = 0; i < 3; i++) {
					for (int j = 0; j < 12; j++) {
						var n = j > 5 ? j + 1 : j;

						if (w == 0) {
							WallTopsA[i * 12 + j] = new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8, n / 5 * 8, 8, 8));
						} else {
							WallTopsB[i * 12 + j] = new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8, 40 + n / 5 * 8, 8, 8));
						}
					}
				}

				for (int i = 0; i < 3; i++) {
					if (w == 0) {
						WallTopsA[i] = new TextureRegion(anim.Texture, new Rectangle(128 + i * 16, 80, 16, 16));
					} else {
						WallTopsB[i] = new TextureRegion(anim.Texture, new Rectangle(128 + i * 16, 96, 16, 16));
					}
				}
			}
			
			WallTopA = new TextureRegion(anim.Texture, new Rectangle(160, 8, 16, 16));
			WallCrackA = new TextureRegion(anim.Texture, new Rectangle(176, 8, 16, 16));
			WallTopB = new TextureRegion(anim.Texture, new Rectangle(160, 8, 48, 16));
			WallCrackB = new TextureRegion(anim.Texture, new Rectangle(176, 8, 48, 16));
			
			for (int f = 0; f < 4; f++) {
				for (int i = 0; i < 16; i++) {
					if (f == 0) {
						FloorA[i] = new TextureRegion(anim.Texture, new Rectangle(i % 4 * 16, 80 + i / 4 * 16, 16, 16));
					} else if (f == 1) {
						FloorB[i] = new TextureRegion(anim.Texture, new Rectangle(64 + i % 4 * 16, 80 + i / 4 * 16, 16, 16));
					} else if (f == 2) {
						FloorA[i] = new TextureRegion(anim.Texture, new Rectangle(i % 4 * 16, 144 + i / 4 * 16, 16, 16));
					} else if (f == 3) {
						FloorB[i] = new TextureRegion(anim.Texture, new Rectangle(64 + i % 4 * 16, 144 + i / 4 * 16, 16, 16));
					}
				}
			}
		}		
	}
}