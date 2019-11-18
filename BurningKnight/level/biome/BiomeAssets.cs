using System;
using BurningKnight.level.tile;
using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.biome {
	public class BiomeAssets {
		public TextureRegion DirtPattern;
		public TextureRegion[] Dirt;
		
		public TextureRegion LavaPattern;
		public TextureRegion[] Lava;
		
		public TextureRegion WaterPattern;
		public TextureRegion[] Water;
		
		public TextureRegion IcePattern;
		public TextureRegion[] Ice;
		
		public TextureRegion VenomPattern;
		public TextureRegion[] Venom;
		
		public TextureRegion GrassPattern;
		public TextureRegion[] Grass;
		
		public TextureRegion CobwebPattern;
		public TextureRegion[] Cobweb;
		
		public TextureRegion ObsidianPattern;
		public TextureRegion[] Obsidian;
		
		public TextureRegion ChasmPattern;
		public TextureRegion[] ChasmTop = new TextureRegion[3];
		public TextureRegion[] ChasmBottom = new TextureRegion[3];
		public TextureRegion[] ChasmLeft = new TextureRegion[3];
		public TextureRegion[] ChasmRight = new TextureRegion[3];
		public TextureRegion[] ChasmSide = new TextureRegion[3];
		
		public TextureRegion EmberPattern;
		
		public TextureRegion[][] Edges = new TextureRegion[(int) Tile.Total][];
		public TextureRegion[] Patterns = new TextureRegion[(int) Tile.Total];

		public TextureRegion[] Planks = new TextureRegion[12];
		public TextureRegion[] PlankTops = new TextureRegion[36];
		public TextureRegion[] PlankSides = new TextureRegion[3];
		public TextureRegion PlanksTop;

		public TextureRegion[] EvilWall = new TextureRegion[12];
		public TextureRegion[] EvilWallTops = new TextureRegion[36];
		public TextureRegion[] EvilWallSides = new TextureRegion[3];
		
		public TextureRegion[] GrannyWall = new TextureRegion[12];
		public TextureRegion[] GrannyWallTops = new TextureRegion[36];
		public TextureRegion[] GrannyWallSides = new TextureRegion[3];
		
		
		public TextureRegion[] GrannyFloor = new TextureRegion[16];
		public TextureRegion[] EvilFloor = new TextureRegion[16];

		public TextureRegion[] Light;

		public BiomeAssets() {
			var anim = Animations.Get("biome_assets");
			Light = GetEdge(anim, 272, 192, 32);

			Patterns[(int) Tile.Dirt] = DirtPattern = new TextureRegion(anim.Texture, new Rectangle(0, 0, 64, 64));
			Edges[(int) Tile.Dirt] = Dirt = GetEdge(anim, 64, 0);

			Patterns[(int) Tile.Lava] = LavaPattern = new TextureRegion(anim.Texture, new Rectangle(0, 64, 64, 64));
			Edges[(int) Tile.Lava] = Lava = GetEdge(anim, 64, 64);

			Patterns[(int) Tile.Water] = WaterPattern = new TextureRegion(anim.Texture, new Rectangle(0, 192, 64, 64));
			Edges[(int) Tile.Water] = Water = GetEdge(anim, 64, 192);

			Patterns[(int) Tile.Ice] = IcePattern = new TextureRegion(anim.Texture, new Rectangle(128, 64, 64, 64));
			Edges[(int) Tile.Ice] = Ice = GetEdge(anim, 192, 64);

			Patterns[(int) Tile.Venom] = VenomPattern = new TextureRegion(anim.Texture, new Rectangle(0, 256, 64, 64));
			Edges[(int) Tile.Venom] = Venom = GetEdge(anim, 64, 256);

			Patterns[(int) Tile.Obsidian] = ObsidianPattern = new TextureRegion(anim.Texture, new Rectangle(0, 128, 64, 64));
			Edges[(int) Tile.Obsidian] = Obsidian = GetEdge(anim, 64, 128);

			Patterns[(int) Tile.HighGrass] = Patterns[(int) Tile.Grass] =
				GrassPattern = new TextureRegion(anim.Texture, new Rectangle(128, 0, 64, 64));

			Edges[(int) Tile.HighGrass] = Edges[(int) Tile.Grass] = Grass = GetEdge(anim, 192, 0);

			Patterns[(int) Tile.Cobweb] = CobwebPattern = new TextureRegion(anim.Texture, new Rectangle(128, 192, 64, 64));
			Edges[(int) Tile.Cobweb] = Cobweb = GetEdge(anim, 192, 192);

			Patterns[(int) Tile.Chasm] = ChasmPattern = new TextureRegion(anim.Texture, new Rectangle(288, 32, 16, 16));
			Patterns[(int) Tile.Ember] = EmberPattern = new TextureRegion(anim.Texture, new Rectangle(128, 128, 64, 64));

			for (int i = 0; i < 3; i++) {
				ChasmTop[i] = new TextureRegion(anim.Texture, new Rectangle(272 + i * 16, 0, 16, 16));
				ChasmBottom[i] = new TextureRegion(anim.Texture, new Rectangle(272 + i * 16, 64, 16, 16));
				ChasmLeft[i] = new TextureRegion(anim.Texture, new Rectangle(256, 16 + i * 16, 16, 16));
				ChasmRight[i] = new TextureRegion(anim.Texture, new Rectangle(320, 16 + i * 16, 16, 16));

				ChasmSide[i] = new TextureRegion(anim.Texture, new Rectangle(256 + i * 16, 80, 16, 16));
			}

			for (int i = 0; i < 3; i++) {
				for (int j = 0; j < 12; j++) {
					var n = j > 5 ? j + 1 : j;

					PlankTops[i * 12 + j] =
						new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8 + i * 40 + 208, n / 5 * 8 + 136, 8, 8));

					EvilWallTops[i * 12 + j] =
						new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8 + i * 40, n / 5 * 8 + 320, 8, 8));

					GrannyWallTops[i * 12 + j] =
						new TextureRegion(anim.Texture, new Rectangle(n % 5 * 8 + i * 40 + 192, n / 5 * 8 + 320, 8, 8));
				}
			}

			for (int i = 0; i < 12; i++) {
				Planks[i] = new TextureRegion(anim.Texture, new Rectangle(i * 16 + 208, 160, 16, 16));
				EvilWall[i] = new TextureRegion(anim.Texture, new Rectangle(i * 16, 344, 16, 16));
				GrannyWall[i] = new TextureRegion(anim.Texture, new Rectangle(i * 16 + 192, 344, 16, 16));
			}

			for (int i = 0; i < 3; i++) {
				PlankSides[i] = new TextureRegion(anim.Texture, new Rectangle(344 + i * 16, 176, 16, 16));
				EvilWallSides[i] = new TextureRegion(anim.Texture, new Rectangle(64 + i * 16, 368, 16, 16));
				GrannyWallSides[i] = new TextureRegion(anim.Texture, new Rectangle(256 + i * 16, 368, 16, 16));
			}

			for (int i = 0; i < 16; i++) {
				EvilFloor[i] = new TextureRegion(anim.Texture, new Rectangle(i % 4 * 16, 368 + i / 4 * 16, 16, 16));
				GrannyFloor[i] = new TextureRegion(anim.Texture, new Rectangle(192 + i % 4 * 16, 368 + i / 4 * 16, 16, 16));
			}

			PlanksTop = new TextureRegion(anim.Texture, new Rectangle(384, 144, 16, 16));
		}

		private static Vector2[] Positions = {
			new Vector2(0, 3), new Vector2(0, 2), new Vector2(1, 3), new Vector2(1, 2),
			new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1),
			new Vector2(3, 3), new Vector2(3, 2), new Vector2(2, 3), new Vector2(2, 2),
			new Vector2(3, 0), new Vector2(3, 1), new Vector2(2, 0), new Vector2(2, 1)
		};

		private TextureRegion[] GetEdge(AnimationData animation, int x, int y, int sz = 16) {
			var edge = new TextureRegion[16];

			for (int i = 0; i < 16; i++) {
				edge[i] = new TextureRegion(animation.Texture, new Rectangle(x + (int) Positions[i].X * sz, y + (int) Positions[i].Y * sz, sz, sz));
			}
			
			return edge;
		}
	}
}