using System;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.level {
	public abstract class Level : SaveableEntity {
		public Tileset Tileset;

		private int width;
		private int height;

		public new int Width {
			get => width;

			set {
				width = value;
				Size = width * height;
			}
		}
		
		public new int Height {
			get => height;

			set {
				height = value;
				Size = width * height;
			}
		}
		
		public int Size;
		public byte[] Tiles;
		public byte[] Liquid;
		public byte[] Variants;
		public byte[] Light;

		public Level() {
			Run.Level = this;
		}

		public override void Init() {
			base.Init();

			Depth = Layers.Floor;
			
			Area.Add(new RenderTrigger(RenderLiquids, Layers.Liquid));
			Area.Add(new RenderTrigger(RenderWalls, Layers.Wall));
		}

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new LevelBodyComponent());

			AlwaysActive = true;
			AlwaysVisible = true;
		}
		
		public void CreateBody() {
			GetComponent<LevelBodyComponent>().CreateBody();
		}

		public void TileUp() {
			LevelTiler.TileUp(this);
		}
		
		public void Set(int i, Tile value) {
			if (value.Matches(TileFlags.LiquidLayer)) {
				Liquid[i] = (byte) value;
			} else {
				Tiles[i] = (byte) value;
			}
		}
		
		public void Set(int x, int y, Tile value) {
			if (value.Matches(TileFlags.LiquidLayer)) {
				Liquid[ToIndex(x, y)] = (byte) value;
			} else {
				Tiles[ToIndex(x, y)] = (byte) value;
			}
		}
		
		public Tile Get(int x, int y, bool liquid = false) {
			return (Tile) (liquid ? Liquid[ToIndex(x, y)] : Tiles[ToIndex(x, y)]);
		}
		
		public Tile Get(int i, bool liquid = false) {
			return (Tile) (liquid ? Liquid[i] : Tiles[i]);
		}
		
		public int ToIndex(int x, int y) {
			return x + y * width;
		}

		public int FromIndexX(int index) {
			return index % width;
		}

		public int FromIndexY(int index) {
			return index / width;
		}

		public bool IsInside(int x, int y) {
			return x >= 0 && y >= 0 && x < width && y < height;
		}

		public bool IsInside(int i) {
			return i >= 0 && i < Size;
		}

		public bool CheckFor(int x, int y, int flag, bool liquid = false) {
			return Get(x, y, liquid).Matches(flag);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			stream.WriteInt32(width);
			stream.WriteInt32(height);

			for (int i = 0; i < Size; i++) {
				stream.WriteByte(Tiles[i]);
				stream.WriteByte(Liquid[i]);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			Width = stream.ReadInt32();
			Height = stream.ReadInt32();

			Setup();

			for (int i = 0; i < Size; i++) {
				Tiles[i] = stream.ReadByte();
				Liquid[i] = stream.ReadByte();
			}
			
			CreateBody();
			TileUp();
		}

		public void Setup() {
			Tiles = new byte[Size];
			Liquid = new byte[Size];
			Variants = new byte[Size];
			Light = new byte[Size];
			
			PathFinder.SetMapSize(Width, Height);
		}

		public override void RenderDebug() {
			Graphics.Batch.DrawRectangle(new RectangleF(0, 0, Width * 16, Height * 16), Color.Green);
		}
		
		protected int GetRenderLeft(Camera camera) {
			return (int) MathUtils.Clamp(0, Width - 1, (int) Math.Floor(camera.X / 16 - 1f));
		}

		protected int GetRenderTop(Camera camera) {
			return (int) MathUtils.Clamp(0, Height - 1, (int) Math.Floor(camera.Y / 16 - 1f));
		}

		protected int GetRenderRight(Camera camera) {
			return (int) MathUtils.Clamp(0, Width - 1, (int) Math.Ceiling(camera.Right / 16 + 1f));
		}

		protected int GetRenderBottom(Camera cameral) {
			return (int) MathUtils.Clamp(0, Height - 1, (int) Math.Ceiling(cameral.Bottom / 16 + 1f));
		}

		// Renders floor layer
		public override void Render() {
			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);

			for (int y = GetRenderTop(camera); y < toY; y++) {
				for (int x = GetRenderLeft(camera); x < toX; x++) {
					var index = ToIndex(x, y);
					var tile = Tiles[index];
					var t = (Tile) tile;

					if (tile > 0 && t.Matches(TileFlags.FloorLayer)) {
						Graphics.Render(t == Tile.Chasm ? Tilesets.Biome.ChasmPattern : Tileset.Tiles[tile][Variants[index]], new Vector2(x * 16, y * 16));
					}
				}
			}
		}

		public bool RenderLiquids() {
			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);

			var region = new TextureRegion();
			
			for (int y = GetRenderTop(camera); y < toY; y++) {
				for (int x = GetRenderLeft(camera); x < toX; x++) {
					var index = ToIndex(x, y);
					var tile = Liquid[index];

					if (tile > 0) {
						region.Set(Tilesets.Biome.Patterns[tile]);
						region.Source.X += x % 4 * 16;
						region.Source.Y += y % 4 * 16;
						region.Source.Width = 16;
						region.Source.Height = 16;

						Graphics.Render(region, new Vector2(x * 16, y * 16));
					}
				}
			}

			return true;
		}
		
		public bool RenderWalls() {
			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);

			for (int y = GetRenderTop(camera); y < toY; y++) {
				for (int x = GetRenderLeft(camera); x < toX; x++) {
					var index = ToIndex(x, y);
					var tile = Tiles[index];
					var t = (Tile) tile;

					if (tile > 0 && t.Matches(TileFlags.WallLayer)) {
						Graphics.Render(Tileset.Tiles[tile][0], new Vector2(x * 16, y * 16 - 8));

						if (!((Tile) Tiles[index + width]).Matches(Tile.WallA, Tile.WallB)) {
							Graphics.Render(t == Tile.WallA ? Tileset.WallA[CalcWallIndex(x, y)] : Tileset.WallB[CalcWallIndex(x, y)], new Vector2(x * 16, y * 16 + 8));
						}
					}
				}
			}

			return true;
		}

		private byte CalcWallIndex(int x, int y) {
			return (byte) ((int) Math.Round(x * 3.5f + y * 2.74f) % 12);
		}
	}
}