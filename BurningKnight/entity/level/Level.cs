using System;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.assets.prefabs;
using BurningKnight.entity.fx;
using BurningKnight.entity.level.biome;
using BurningKnight.entity.level.tile;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.file;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.level {
	public abstract class Level : SaveableEntity {
		public Tileset Tileset;
		public Biome Biome;

		private int width;
		private int height;
		private float time;

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
		public byte[] LiquidVariants;
		public byte[] Light;
		public byte[] Flags;

		public Chasm Chasm;

		public Level(BiomeInfo biome) {
			SetBiome(biome);

			if (!(this is Prefab)) {
				Run.Level = this;
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (Run.Level == this) {
				Run.Level = null;
			}
		}

		protected void SetBiome(BiomeInfo biome) {
			if (biome != null) {
				Biome = (Biome) Activator.CreateInstance(biome.Type);
				Tileset = Tilesets.Get(Biome.Tileset);
			}
		}

		public override void Init() {
			base.Init();

			Depth = Layers.Floor;
			
			Chasm = new Chasm {
				Level = this
			};
			
			Area.Add(Chasm);
			Area.Add(new RenderTrigger(this, RenderLiquids, Layers.Liquid));
			Area.Add(new RenderTrigger(this, RenderWalls, Layers.Wall));
			Area.Add(new RenderTrigger(this, Lights.Render, Layers.Light));
		}

		public override void AddComponents() {
			base.AddComponents();
			AddComponent(new LevelBodyComponent());

			AlwaysActive = true;
			AlwaysVisible = true;
		}
		
		public void CreateBody() {
			if (components == null) {
				return;
			}
			
			GetComponent<LevelBodyComponent>().CreateBody();
			Chasm.GetComponent<ChasmBodyComponent>().CreateBody();	
		}

		public void TileUp() {
			LevelTiler.TileUp(this);
		}
		
		public void UpdateTile(int x, int y) {
			var i = ToIndex(x, y);
			Variants[i] = 0;
			
			foreach (var d in PathFinder.Neighbours9) {
				var index = i + d;
				
				if (IsInside(index)) {
					LevelTiler.TileUp(this, index);	
				}
			}
		}

		public void Fill(Tile tile) {
			byte t = (byte) tile;

			for (int i = 0; i < Size; i++) {
				Tiles[i] = t;
			}
		}
				
		public void Set(int i, Tile value) {
			if (value.Matches(TileFlags.LiquidLayer)) {
				Liquid[i] = (byte) value;
			} else {
				if (value.Matches(Tile.WallA, Tile.WallB)) {
					Liquid[i] = 0;
				}
				
				Tiles[i] = (byte) value;
			}
		}
		
		public void Set(int x, int y, Tile value) {
			Set(ToIndex(x, y), value);
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
			
			stream.WriteString(Biome.Id);
			stream.WriteInt32(width);
			stream.WriteInt32(height);

			for (int i = 0; i < Size; i++) {
				stream.WriteByte(Tiles[i]);
				stream.WriteByte(Liquid[i]);
				stream.WriteByte(Flags[i]);
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			
			SetBiome(BiomeRegistry.Defined[stream.ReadString()]);

			Width = stream.ReadInt32();
			Height = stream.ReadInt32();

			Setup();

			for (int i = 0; i < Size; i++) {
				Tiles[i] = stream.ReadByte();
				Liquid[i] = stream.ReadByte();
				Flags[i] = stream.ReadByte();
			}
			
			CreateBody();
			TileUp();
		}

		public void Setup() {
			Tiles = new byte[Size];
			Liquid = new byte[Size];
			Variants = new byte[Size];
			LiquidVariants = new byte[Size];
			Light = new byte[Size];
			Flags = new byte[Size];
			
			PathFinder.SetMapSize(Width, Height);
		}

		public bool CheckFlag(int x, int y, int i) {
			return CheckFlag(ToIndex(x, y), i);
		}
		
		public bool CheckFlag(int index, int i) {
			return BitHelper.IsBitSet(Flags[index], i);
		}

		public void SetFlag(int x, int y, int i, bool on) {
			SetFlag(ToIndex(x, y), i, on);
		}

		public void SetFlag(int index, int i, bool on) {
			Flags[index] = (byte) BitHelper.SetBit(Flags[index], i, on);
		}

		public override void RenderDebug() {
			Graphics.Batch.DrawRectangle(new RectangleF(0, 0, Width * 16, Height * 16), Color.Green);
		}
		
		public int GetRenderLeft(Camera camera) {
			return (int) MathUtils.Clamp(0, Width - 1, (int) Math.Floor(camera.X / 16 - 1f));
		}

		public int GetRenderTop(Camera camera) {
			return (int) MathUtils.Clamp(0, Height - 1, (int) Math.Floor(camera.Y / 16 - 1f));
		}

		public int GetRenderRight(Camera camera) {
			return (int) MathUtils.Clamp(0, Width - 1, (int) Math.Ceiling(camera.Right / 16 + 1f));
		}

		public int GetRenderBottom(Camera camera) {
			return (int) MathUtils.Clamp(0, Height - 1, (int) Math.Ceiling(camera.Bottom / 16 + 1f));
		}

		public override void Update(float dt) {
			base.Update(dt);
			time += dt;
		}

		// Renders floor layer
		public override void Render() {
			if (this != Run.Level) {
				Done = true;
				return;
			}
			
			var camera = Camera.Instance;
			var paused = Engine.Instance.State.Paused;
			
			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderTop(camera);

			var shader = Shaders.Chasm;
			Shaders.Begin(shader);

			shader.Parameters["h"].SetValue(8f / Tileset.WallTopA.Texture.Height);
			var sy = shader.Parameters["y"];
			var enabled = shader.Parameters["enabled"];
			enabled.SetValue(false);
							
			for (int y = GetRenderBottom(camera); y >= toY; y--) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);
					var tile = Tiles[index];
					var t = (Tile) tile;

					if (tile > 0) {
						if (t.Matches(TileFlags.FloorLayer)) {
							var pos = new Vector2(x * 16, y * 16);

							if (t == Tile.Chasm) {
								Graphics.Render(Tilesets.Biome.ChasmPattern, pos);

								if (!paused && Random.Chance(0.1f)) {
									Area.Add(new ChasmFx {
										Position = pos + new Vector2(Random.Float(16), Random.Float(16))
									});
								}

								if (index >= width) {
									var tt = Get(index - width);

									if (tt != Tile.Chasm) {
										var ind = CalcWallSide(x, y);
										TextureRegion region;

										switch (tt) {
											case Tile.WallA:
												region = Tileset.WallA[ind];
												break;
											case Tile.FloorA:
												region = Tileset.FloorSidesA[ind];
												break;
											case Tile.FloorB:
												region = Tileset.FloorSidesB[ind];
												break;
											case Tile.FloorC:
												region = Tileset.FloorSidesC[ind];
												break;
											case Tile.FloorD:
												region = Tileset.FloorSidesD[ind];
												break;
											default:
											case Tile.WallB:
												region = Tileset.WallB[ind];
												break;
										}

										enabled.SetValue(true);
										sy.SetValue((float) region.Source.Y / Tileset.WallTopA.Texture.Height);
										Graphics.Render(region, pos);
										enabled.SetValue(false);
										
										var id = -1;

										if (!IsInside(index + 1 - width) || ((Tile) Tiles[index + 1 - width]).Matches(Tile.Chasm)) {
											id += 1;
										}
							
										if (!IsInside(index - 1 - width) || ((Tile) Tiles[index - 1 - width]).Matches(Tile.Chasm)) {
											id += 2;
										}

										if (id != -1) {
											Graphics.Render(Tilesets.Biome.ChasmSide[id], pos);
										}
									}
								}
							} else {
								Graphics.Render(Tileset.Tiles[tile][Variants[index]], pos);
							}
						} else if (t.Matches(TileFlags.WallLayer) && (!IsInside(index + width) || !((Tile) Tiles[index + width]).Matches(Tile.WallA, Tile.WallB))) {
							var pos = new Vector2(x * 16, y * 16 + 8);
							Graphics.Render(t == Tile.WallA ? Tileset.WallA[CalcWallIndex(x, y)] : Tileset.WallB[CalcWallIndex(x, y)], pos);

							var ind = -1;

							if (index >= Size - 1 || !((Tile) Tiles[index + 1]).Matches(Tile.WallA, Tile.WallB)) {
								ind += 1;
							}
							
							if (index <= 0 || !((Tile) Tiles[index - 1]).Matches(Tile.WallA, Tile.WallB)) {
								ind += 2;
							}

							if (ind != -1) {
								Graphics.Render(t == Tile.WallA ? Tileset.WallSidesA[ind] : Tileset.WallSidesB[ind], pos);
							}
						}
					}
				}
			}

			Shaders.End();
		}

		public void RenderLiquids() {
			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);

			var region = new TextureRegion();
			var shader = Shaders.Terrain;
			
			Shaders.Begin(shader);

			var paused = Engine.Instance.State.Paused;
			
			var enabled = shader.Parameters["enabled"];
			var tilePosition = shader.Parameters["tilePosition"];
			var edgePosition = shader.Parameters["edgePosition"];
			var flow = shader.Parameters["flow"];
			flow.SetValue(0f);
			
			shader.Parameters["time"].SetValue(time * 0.04f);
			shader.Parameters["h"].SetValue(64f / Tilesets.Biome.WaterPattern.Texture.Height);

			var sy = shader.Parameters["sy"];

			enabled.SetValue(true);

			for (int y = GetRenderTop(camera); y <= toY; y++) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);
					var tile = Liquid[index];

					if (tile > 0) {
						var tt = (Tile) tile;

						region.Set(Tilesets.Biome.Patterns[tile]);
						region.Source.Width = 16;
						region.Source.Height = 16;

						if (tt == Tile.Water) {
							flow.SetValue(1f);
							sy.SetValue(y % 4 * 16f / Tilesets.Biome.WaterPattern.Texture.Height);
						} else {
							region.Source.Y += y % 4 * 16;
						}

						region.Source.X += x % 4 * 16;
						
						var pos = new Vector2(x * 16, y * 16);

						var t = (Tile) tile;
						
						if (!t.Matches(Tile.Ember, Tile.Chasm)) {
							var edge = Tilesets.Biome.Edges[tile][LiquidVariants[index]];

							edgePosition.SetValue(new Vector2(
								(float) edge.Source.X / edge.Texture.Width,
								(float) edge.Source.Y / edge.Texture.Height
							));
							
							tilePosition.SetValue(new Vector2(
								(float) region.Source.X / region.Texture.Width,
								(float) region.Source.Y / region.Texture.Height
							));
							
							Graphics.Render(region, pos);

							if (t == Tile.Water) {
								if (!paused && Get(index + width) == Tile.Chasm && Random.Chance(10)) {
									Area.Add(new WaterfallFx {
										Position = pos + new Vector2(Random.Float(16), 16)
									});
								}
							} else if (t == Tile.HighGrass) {
								enabled.SetValue(false);

								var tm = Engine.Instance.State.Time;
								
								Graphics.Render(Tilesets.Biome.HighGrass, new Vector2(x * 16 + 8, y * 16 + 16), 
									(float) (Math.Cos(tm - y * Math.PI * 0.25f) * Math.Sin(tm * 0.9f + x * Math.PI * 0.3f) * 0.6f),
									new Vector2(8, 16));
								
								enabled.SetValue(true);
							}
						} else {
							enabled.SetValue(false);
							Graphics.Render(region, pos);
							enabled.SetValue(true);
						}
						
						if (tt == Tile.Water) {
							flow.SetValue(0f);
						}
					}
				}
			}


			for (int y = GetRenderTop(camera); y < toY; y++) {
				for (int x = GetRenderLeft(camera); x < toX; x++) {
					var index = ToIndex(x, y);
					
					if (Get(index) == Tile.Chasm) {
						enabled.SetValue(false);
						
						if (IsInside(index + width) && Get(index + width) != Tile.Chasm) {
							Graphics.Render(Tilesets.Biome.ChasmBottom[CalcWallTopIndex(x, y + 1)], new Vector2(x * 16, y * 16 + 16));
						}

						/*if (Get(index - width) != Tile.Chasm) {
							Graphics.Render(Tilesets.Biome.ChasmTop[CalcWallTopIndex(x, y - 1)], new Vector2(x * 16, y * 16 - 16));
						}*/
								
						if (IsInside(index + 1) && Get(index + 1) != Tile.Chasm) {
							Graphics.Render(Tilesets.Biome.ChasmRight[CalcWallTopIndex(x + 1, y)], new Vector2(x * 16 + 16, y * 16));
						}
								
						if (index > 0 && Get(index - 1) != Tile.Chasm) {
							Graphics.Render(Tilesets.Biome.ChasmLeft[CalcWallTopIndex(x - 1, y)], new Vector2(x * 16 - 16, y * 16));
						}
						
						enabled.SetValue(true);
					}
				}
			}

			Shaders.End();
		}
		
		public void RenderWalls() {
			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);

			for (int y = GetRenderTop(camera); y <= toY; y++) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);
					var tile = Tiles[index];
					var t = (Tile) tile;

					if (tile > 0 && t.Matches(TileFlags.WallLayer)) {
						Graphics.Render(Tileset.Tiles[tile][0], new Vector2(x * 16, y * 16 - 8));

						if (t.Matches(Tile.WallA, Tile.WallB)) {
							byte v = Variants[index];
							
							for (int xx = 0; xx < 2; xx++) {
								for (int yy = 0; yy < 2; yy++) {
									int lv = 0;

									if (yy > 0 || BitHelper.IsBitSet(v, 0)) {
										lv += 1;
									}

									if (xx == 0 || BitHelper.IsBitSet(v, 1)) {
										lv += 2;
									}

									if (yy == 0 || BitHelper.IsBitSet(v, 2)) {
										lv += 4;
									}

									if (xx > 0 || BitHelper.IsBitSet(v, 3)) {
										lv += 8;
									}

									if (lv == 15) {
										lv = 0;

										if (xx == 1 && yy == 0 && !BitHelper.IsBitSet(v, 4)) {
											lv += 1;
										}

										if (xx == 1 && yy == 1 && !BitHelper.IsBitSet(v, 5)) {
											lv += 2;
										}

										if (xx == 0 && yy == 1 && !BitHelper.IsBitSet(v, 6)) {
											lv += 4;
										}

										if (xx == 0 && yy == 0 && !BitHelper.IsBitSet(v, 7)) {
											lv += 8;
										}

										var vl = Tileset.wallMapExtra[lv];

										if (vl != -1) {
											Graphics.Render(t == Tile.WallA ? Tileset.WallTopsA[vl + 12 * CalcWallTopIndex(x, y)] : Tileset.WallTopsB[vl + 12 * CalcWallTopIndex(x, y)], new Vector2(x * 16 + xx * 8, y * 16 + yy * 8 - 8));
										}
									} else {
										var vl = Tileset.wallMap[lv];
										
										if (vl != -1) {
											Graphics.Render(t == Tile.WallA ? Tileset.WallTopsA[vl + 12 * CalcWallTopIndex(x, y)] : Tileset.WallTopsB[vl + 12 * CalcWallTopIndex(x, y)], new Vector2(x * 16 + xx * 8, y * 16 + yy * 8 - 8));
										}
									}
								}
							}
						}
					}
				}
			}
		}

		private byte CalcWallIndex(int x, int y) {
			return (byte) (((int) Math.Round(x * 3.5f + y * 2.74f)) % 12);
		}

		private byte CalcWallSide(int x, int y) {
			return (byte) (((int) Math.Round(x * 3.5f + y * 2.74f)) % 4);
		}

		private byte CalcWallTopIndex(int x, int y) {
			return (byte) (((int) Math.Round(x * 16.217f + y * 8.12f)) % 3);
		}

		public virtual Tile GetFilling() {
			return Tile.WallA;
		}
	}
}