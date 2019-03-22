using System;
using BurningKnight.entity.level;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.editor {
	public class TileSelect : Entity {
		public Tile Current = Tile.FloorA;
		public int CurrentIndex = 4;
		public Editor Editor;

		private Tile[] order = {
			Tile.WallA,
			Tile.WallB,
			Tile.Crack,
			
			Tile.FloorA,
			Tile.FloorB,
			Tile.FloorC,
			Tile.FloorD,
			
			Tile.Dirt,
			Tile.Grass,
			Tile.HighGrass,
			Tile.Water,
			Tile.Lava,
			Tile.Chasm,
			Tile.Cobweb,
			Tile.Obsidian,
			Tile.Ember,
			Tile.Ice,
			Tile.Venom
		};

		private TextureRegion[] icons;
		
		public override void Init() {
			base.Init();

			var set = Editor.Level.Tileset;
			var biome = Tilesets.Biome;

			Depth = 1;
			Width = 20;
			Height = Display.UiHeight;
			
			icons = new[] {
				set.WallTopA,
				set.WallTopB,
				set.WallCrackA,
				
				set.FloorA[0],
				set.FloorB[0],
				set.FloorC[0],
				set.FloorD[0],
				
				new TextureRegion(biome.DirtPattern, 16, 16),
				new TextureRegion(biome.GrassPattern, 16, 16),
				new TextureRegion(biome.GrassPattern, 16, 16), // fixme: high grass
				new TextureRegion(biome.WaterPattern, 16, 16),
				new TextureRegion(biome.LavaPattern, 16, 16),
				new TextureRegion(biome.ChasmPattern, 16, 16),
				new TextureRegion(biome.CobwebPattern, 16, 16),
				new TextureRegion(biome.ObsidianPattern, 16, 16),
				new TextureRegion(biome.EmberPattern, 16, 16),
				new TextureRegion(biome.IcePattern, 16, 16),
				new TextureRegion(biome.VenomPattern, 16, 16)
			};

			AlwaysActive = true;
			AlwaysVisible = true;
		}

		public bool OnClick(Vector2 pos) {
			if (!Editor.ShowPanes || !Contains(Input.Mouse.UiPosition)) {
				return false;	
			}

			CurrentIndex = (int) Math.Floor((Input.Mouse.UiPosition.Y + 1) / 18);
			Current = order[CurrentIndex];

			return true;
		}

		public override void Render() {
			Graphics.Batch.FillRectangle(new RectangleF(0, 0, 20, Display.UiHeight), Color.Black);
			var pos = Input.Mouse.UiPosition;
			
			for (int i = 0; i < icons.Length; i++) {
				if (i == CurrentIndex) {
					Graphics.Batch.FillRectangle(new RectangleF(1, i * 18, 18, 18), ColorUtils.WhiteColor);
				} else if (new Rectangle(0, i * 18 + 1, 20, 18).Contains(pos)) {
					Graphics.Batch.FillRectangle(new RectangleF(1, i * 18, 18, 18), Color.Wheat);
					Graphics.Batch.FillRectangle(new RectangleF(1, i * 18, 18, 18), Color.B);acl
				}
				
				Graphics.Render(icons[i], new Vector2(2, i * 18 + 1));
			}
		}
	}
}