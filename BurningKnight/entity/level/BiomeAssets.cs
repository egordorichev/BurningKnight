using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level {
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
		public TextureRegion[] Chasm;
		
		public TextureRegion EmberPattern;
		
		public TextureRegion[][] Edges = new TextureRegion[(int) Tile.Total][];
		public TextureRegion[] Patterns = new TextureRegion[(int) Tile.Total];
		
		public BiomeAssets() {
			var anim = Animations.Get("biome_assets");
			
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
			
			Patterns[(int) Tile.Grass] = GrassPattern = new TextureRegion(anim.Texture, new Rectangle(128, 0, 64, 64));
			Edges[(int) Tile.Grass] = Grass = GetEdge(anim, 192, 0);
			
			Patterns[(int) Tile.Cobweb] = CobwebPattern = new TextureRegion(anim.Texture, new Rectangle(128, 192, 64, 64));
			Edges[(int) Tile.Cobweb] = Cobweb = GetEdge(anim, 192, 192);
			
			Patterns[(int) Tile.Chasm] = ChasmPattern = new TextureRegion(anim.Texture, new Rectangle(288, 32, 16, 16));
			Patterns[(int) Tile.Ember] = EmberPattern = new TextureRegion(anim.Texture, new Rectangle(128, 128, 64, 64));
		}

		private static Vector2[] Positions = {
			new Vector2(0, 3), new Vector2(0, 2), new Vector2(3, 1), new Vector2(2, 1),
			new Vector2(1, 3), new Vector2(1, 2), new Vector2(1, 1), new Vector2(0, 0), 
			new Vector2(0, 1), new Vector2(1, 0), new Vector2(1, 1), new Vector2(3, 3), 
			new Vector2(3, 2), new Vector2(2, 3), new Vector2(2, 2), new Vector2(3, 2)
		};

		private TextureRegion[] GetEdge(AnimationData animation, int x, int y) {
			var edge = new TextureRegion[16];

			for (int i = 0; i < 16; i++) {
				edge[i] = new TextureRegion(animation.Texture, new Rectangle(x + (int) Positions[i].X * 16, y + (int) Positions[i].Y * 16, 16, 16));
			}
			
			return edge;
		}
	}
}