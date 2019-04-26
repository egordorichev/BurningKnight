using System;
using BurningKnight.level.tile;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BurningKnight.entity.editor {
	public class TileSelect : Entity {
		public Tile Current = Tile.FloorA;
		public int CurrentIndex = 3;
		public Editor Editor;

		private float offset;
		
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

		public override void Update(float dt) {
			base.Update(dt);

			float speed = 320 * dt;

			if (Input.Keyboard.IsDown(Keys.Down)) {
				offset -= speed;
			} else if (Input.Keyboard.IsDown(Keys.Up)) {
				offset += speed;
			}

			offset = MathUtils.Clamp(-icons.Length * 20 + 34 + Display.UiHeight, 0, offset);
		}

		public bool OnClick(Vector2 pos) {
			if (!Editor.ShowPanes || !Contains(Input.Mouse.UiPosition)) {
				return false;	
			}

			CurrentIndex = (int) MathUtils.Clamp(0, order.Length - 1, (float) 
				Math.Floor((Input.Mouse.UiPosition.Y - offset + 1) / 18));
			Current = order[CurrentIndex];

			return true;
		}

		public override void Render() {
			Graphics.Batch.FillRectangle(new RectangleF(0, 0, 20, Display.UiHeight), Color.Black);
			var pos = Input.Mouse.UiPosition;
			
			for (int i = 0; i < icons.Length; i++) {
				if (i == CurrentIndex) {
					Graphics.Batch.FillRectangle(new RectangleF(1, i * 18 + offset, 18, 18), ColorUtils.WhiteColor);
					Graphics.Batch.FillRectangle(new RectangleF(2, i * 18 + 1 + offset, 16, 16), Color.Black);
				} else if (new Rectangle(0, (int) (i * 18 + 1 + offset), 20, 18).Contains(pos)) {
					Graphics.Batch.FillRectangle(new RectangleF(1, i * 18 + offset, 18, 18), Color.Red);
					Graphics.Batch.FillRectangle(new RectangleF(2, i * 18 + 1 + offset, 16, 16), Color.Black);
				}
				
				Graphics.Render(icons[i], new Vector2(2, i * 18 + 1 + offset));
			}
		}
	}
}