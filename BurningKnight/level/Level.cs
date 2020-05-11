// #define ART_DEBUG

using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.custom;
using BurningKnight.debug;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.fx;
using BurningKnight.entity.room;
using BurningKnight.level.biome;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.level.variant;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.util;
using Lens.util.camera;
using Lens.util.file;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace BurningKnight.level {
	public abstract class Level : SaveableEntity {
		public const float LightMin = 0.01f;
		public const float LightMax = 0.95f;
		public static bool RenderPassable = false;
		public static Color ShadowColor = new Color(0f, 0f, 0f, 0.5f);
		public static Color FloorColor = new Color(1f, 1f, 1f, 1f);
		
		public Tileset Tileset;
		public Tileset MatrixTileset;
		public Biome Biome;
		public bool DrawLight = true;
		public bool NoLightNoRender = true;
		public bool Dark;
		public bool Rains;
		public bool Snows;

		public List<string> ItemsToSpawn;

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
		public byte[] Flags;
		public byte[] WallDecor;
		public bool[] Explored;
		public bool[] Passable;
		public bool[] MatrixLeak;
		public float[] Light;

		public Chasm Chasm;
		public HalfWall HalfWall;
		public HalfProjectileLevel HalfProjectile;
		public ProjectileLevelBody ProjectileLevelBody;
		public RenderTarget2D WallSurface;
		public RenderTarget2D MessSurface;

		public LevelVariant Variant;

		public Level(BiomeInfo biome) {
			SetBiome(biome);
			
			Run.Level = this;
		}

		public override void Destroy() {
			base.Destroy();

			rainSound?.Stop();

			if (Chasm != null) {
				HalfProjectile.Done = true;
				Area.Remove(HalfProjectile);
				
				HalfWall.Done = true;
				Area.Remove(HalfWall);

				Chasm.Done = true;
				Area.Remove(Chasm);

				ProjectileLevelBody.Done = true;
				Area.Remove(ProjectileLevelBody);
			}

			if (Run.Level == this) {
				Run.Level = null;
			}

			if (WallSurface != null) {
				WallSurface?.Dispose();
				MessSurface?.Dispose();
			}

			manager?.Destroy();
		}

		public void SetBiome(BiomeInfo biome) {
			if (biome != null) {
				Biome = (Biome) Activator.CreateInstance(biome.Type);
				Tileset = Tilesets.Get(Biome.Tileset);

				if (Tilesets.Biome != null && Tileset != null) {
					Tileset.Tiles[(int) Tile.EvilFloor] = Tilesets.Biome.EvilFloor;
					Tileset.Tiles[(int) Tile.GrannyFloor] = Tilesets.Biome.GrannyFloor;
					Tileset.Tiles[(int) Tile.EvilWall] = Tileset.Tiles[(int) Tile.WallA];
					Tileset.Tiles[(int) Tile.GrannyWall] = Tileset.Tiles[(int) Tile.WallA];
				}

				Biome.Apply();
			}

			if (MatrixTileset == null) {
				MatrixTileset = Tilesets.Get("tech_biome");
			}
		}

		private RenderTriggerManager manager;

		public override void Init() {
			base.Init();
			
			NoLightNoRender = Engine.Instance.State is InGameState;
			Variant = new RegularLevelVariant();
			
			var s = BlendState.AlphaBlend;
				
			blend = new BlendState {
				BlendFactor = s.BlendFactor,
				AlphaDestinationBlend = s.AlphaDestinationBlend,
				ColorDestinationBlend = s.ColorDestinationBlend,
				ColorSourceBlend = s.ColorSourceBlend,
				AlphaBlendFunction = s.AlphaBlendFunction,
				ColorBlendFunction = s.ColorBlendFunction,
					
				AlphaSourceBlend = Blend.DestinationAlpha
			};

			messBlend = new BlendState {
				ColorBlendFunction = BlendFunction.Add,
				ColorSourceBlend = Blend.DestinationColor,
				ColorDestinationBlend = Blend.Zero,
					
				AlphaSourceBlend = Blend.DestinationAlpha
			};

			Depth = Layers.Floor;
		}

		public override void PostInit() {
			base.PostInit();

			manager = new RenderTriggerManager(this);
			
			manager.Add(new RenderTrigger(this, RenderChasms, Layers.Chasm));
			manager.Add(new RenderTrigger(this, RenderLiquids, Layers.Liquid));
			manager.Add(new RenderTrigger(this, RenderSides, Layers.Sides));
			manager.Add(new RenderTrigger(this, RenderWalls, Layers.Wall));
			manager.Add(new RenderTrigger(this, Lights.Render, Layers.Light));
			manager.Add(new RenderTrigger(this, RenderLight, Layers.TileLights));
			manager.Add(new RenderTrigger(this, RenderShadowSurface, Layers.Shadows));
			manager.Add(new RenderTrigger(this, RenderRocks, Layers.Rocks));
		}

		private SoundEffectInstance rainSound;
		
		public void Prepare() {
			try {
				Variant?.PostInit(this);

				if (Dark) {
					Lights.ClearColor = new Color(0f, 0f, 0f, 1f);
				}

				if (Rains) {
					for (var i = 0; i < 40; i++) {
						Run.Level.Area.Add(new RainParticle());
					}

					var sound = "level_rain_regular";

					if (Biome is IceBiome) {
						sound = "level_rain_snow";
					} else if (Biome is JungleBiome) {
						sound = "level_rain_jungle";
					}

					var s = Audio.GetSfx(sound);

					if (s != null) {
						rainSound = s.CreateInstance();

						if (rainSound != null) {
							rainSound.Volume = 0;
							rainSound.IsLooped = true;
							rainSound.Play();

							Tween.To(0.5f, 0, x => rainSound.Volume = x, 1f).Delay = 3f;
						}
					}
				}

				if (Snows) {
					for (var i = 0; i < 120; i++) {
						Run.Level.Area.Add(new SnowParticle());
					}
				}
			} catch (Exception e) {
				Log.Error(e);
			}

			TileUp();
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new LevelBodyComponent {
				Level = this
			});
			
			AddComponent(new ShadowComponent(RenderShadows));

			AlwaysActive = true;
			AlwaysVisible = true;
		}

		public void ReCreateBodyChunk(int x, int y) {
			if (Components == null) {
				return;
			}

			MarkForBodyUpdate(x, y);
			
			if (x % LevelBodyComponent.ChunkSize == 0) {
				MarkForBodyUpdate(x - 1, y);
			} else if (x % LevelBodyComponent.ChunkSize == LevelBodyComponent.ChunkSize - 1) {
				MarkForBodyUpdate(x + 1, y);
			}
			
			if (y % LevelBodyComponent.ChunkSize == 0) {
				MarkForBodyUpdate(x, y - 1);
			} else if (y % LevelBodyComponent.ChunkSize == LevelBodyComponent.ChunkSize - 1) {
				MarkForBodyUpdate(x, y + 1);
			}
		}
		
		private void MarkForBodyUpdate(int x, int y) {
			GetComponent<LevelBodyComponent>().ReCreateBodyChunk(x, y);
			Chasm.GetComponent<ChasmBodyComponent>().ReCreateBodyChunk(x, y);
			HalfWall.GetComponent<HalfWallBodyComponent>().ReCreateBodyChunk(x, y);
			HalfProjectile.GetComponent<HalfProjectileBodyComponent>().ReCreateBodyChunk(x, y);
			ProjectileLevelBody.GetComponent<ProjectileBodyComponent>().ReCreateBodyChunk(x, y);		
		}
		
		public void CreateBody() {
			if (Components == null) {
				return;
			}
			
			if (Chasm == null) {
				Area.Add(HalfWall = new HalfWall {
					Level = this
				});
				
				Area.Add(HalfProjectile = new HalfProjectileLevel {
					Level = this
				});
				
				Area.Add(Chasm = new Chasm {
					Level = this
				});

				Area.Add(ProjectileLevelBody = new ProjectileLevelBody {
					Level = this
				});
			}
			
			Chasm.GetComponent<ChasmBodyComponent>().CreateBody();			
			HalfWall.GetComponent<HalfWallBodyComponent>().CreateBody();
			HalfProjectile.GetComponent<HalfProjectileBodyComponent>().CreateBody();
			ProjectileLevelBody.GetComponent<ProjectileBodyComponent>().CreateBody();
			GetComponent<LevelBodyComponent>().CreateBody();
		}

		public void CreateDestroyableBody() {
			if (Components == null) {
				return;
			}
		
			// Destroyable.GetComponent<DestroyableBodyComponent>().CreateBody();
		}

		private bool loadMarked;
		private bool first;
		private BlendState blend;
		private BlendState messBlend;

		public void LoadPassable() {
			if (!first) {
				first = true;
				CreatePassable();
			} else {
				loadMarked = true;
			}

			var rooms = Area.Tagged[Tags.Room];
			var f = GetFilling() == Tile.Chasm;
			
			for (var i = 0; i < Size - Width; i++) {
				var found = false;
				var x = FromIndexX(i);
				var y = FromIndexY(i);
				
				foreach (var r in rooms) {
					var room = (Room) r;
					
					if (room.ContainsTile(x, y, 1)) {
						if (!f || room.Type != RoomType.Connection) {
							found = true;
						}

						break;
					}
					
					//if (Get(i).Matches(Tile.WallA, Tile.Transition) && Get(i + Width).Matches(Tile.WallA, Tile.Transition)) {
				}

				if (!found) {
					foreach (var d in Area.Tagged[Tags.Door]) {
						if ((int) Math.Floor(d.CenterX / 16) == x) {
							var yy = (int) Math.Floor(d.CenterY / 16);

							if (yy == y + 1 || yy == y) {
								found = true;
							}

							break;
						}
					}
				}

				if (!found) {
					Explored[i] = true;
					Light[i] = 1f;
				}
			}
		}

		public bool IsPassable(int x, int y) {
			return IsPassable(ToIndex(x, y));
		}

		public bool IsPassable(int i, bool chasm = false) {
			var t = Get(i);

			if (Biome is IceBiome && (t == Tile.WallA || t == Tile.Transition)) {
				return true;
			}

			if (chasm && t == Tile.Chasm) {
				return true;
			}
			
			return t.Matches(TileFlags.Passable) && (Liquid[i] == 0 || Get(i, true).Matches(TileFlags.Passable));
		}

		public void CreatePassable(bool chasm = false) {
			for (var i = 0; i < Size; i++) {
				Passable[i] = IsPassable(i, chasm);
			}
		}

		public void TileUp(bool full = false) {
			cleared = false;
			Size = width * height;
			PathFinder.SetMapSize(Width, Height);
			LevelTiler.TileUp(this, full);
		}
		
		public void UpdateTile(int x, int y) {
			var i = ToIndex(x, y);
			LevelTiler.TileUp(this, i);

			for (var xx = -3; xx <= 2; xx++) {
				for (var yy = -3; yy <= 2; yy++) {
					var index = ToIndex(xx + x, yy + y);
					
					if (IsInside(index)) {
						Variants[index] = 0;
						LiquidVariants[index] = 0;
						LevelTiler.TileUp(this, index);	
					}
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
				var t = Get(i);
				
				if (t == Tile.Chasm) {
					return;
				}

				if (t.IsWall() || t.Matches(Tile.SensingSpikeTmp) || t.Matches(Tile.SpikeOnTmp) || t.Matches(Tile.SpikeOnTmp) || t.Matches(Tile.Plate)) {
					Tiles[i] = (byte) Tile.FloorA;
					Variants[i] = 0;
				}

				Liquid[i] = (byte) value;
				LiquidVariants[i] = 0;
			} else {
				if (value.IsWall() || value == Tile.Chasm || ((Tile) Liquid[i]).Matches(Tile.Lava, Tile.Rock, Tile.TintedRock, Tile.MetalBlock)) {
					Liquid[i] = 0;
					LiquidVariants[i] = 0;
				}
				
				Tiles[i] = (byte) value;
				Variants[i] = 0;
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
				stream.WriteBoolean(Explored[i]);
			}	
			
			stream.WriteBoolean(Dark);
			stream.WriteBoolean(Snows);
			stream.WriteBoolean(Rains);

			if (Variant == null) {
				Variant = new RegularLevelVariant();
			}
			
			stream.WriteString(Variant.Id);
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			var biome = stream.ReadString();

			if (BiomeRegistry.Defined.TryGetValue(biome, out var b)) {
				SetBiome(b);
			} else {
				SetBiome(BiomeRegistry.Defined[Biome.Castle]);
			}
			
			Width = stream.ReadInt32();
			Height = stream.ReadInt32();

			Setup();

			for (var i = 0; i < Size; i++) {
				Tiles[i] = stream.ReadByte();
				Liquid[i] = stream.ReadByte();
				Flags[i] = stream.ReadByte();
				Explored[i] = stream.ReadBoolean();
			}
			
			CreateBody();
			CreateDestroyableBody();
			
			Dark = stream.ReadBoolean();
			Snows = stream.ReadBoolean();
			Rains = stream.ReadBoolean();

			Variant = VariantRegistry.Create(stream.ReadString());
			LoadPassable();
		}

		public void MarkForClearing() {
			Tiles = new byte[Size];
			Liquid = new byte[Size];
			Variants = new byte[Size];
			LiquidVariants = new byte[Size];
			cleared = false;
		}

		public void Setup() {
			Size = width * height;
			
			Tiles = new byte[Size];
			Liquid = new byte[Size];
			Variants = new byte[Size];
			LiquidVariants = new byte[Size];
			Light = new float[Size];
			Flags = new byte[Size];
			WallDecor = new byte[Size];
			Explored = new bool[Size];

			var light = Run.Depth == 0;
			
			for (var i = 0; i < Size; i++) {
				if (Rnd.Chance(10)) {
					WallDecor[i] = (byte) Rnd.Int(1, 9);
				}

				if (light) {
					Explored[i] = true;
					Light[i] = 1;
				}
			}
			
			Passable = new bool[Size];
			MatrixLeak = new bool[Size];

			/*if (Run.Depth > 0) {
				for (var i = 0; i < Size; i++) {
					if (Rnd.Chance(1)) {
						MatrixLeak[i] = true;
					}
				}
			}*/

			PathFinder.SetMapSize(Width, Height);
			
			WallSurface = new RenderTarget2D(Engine.GraphicsDevice, Display.Width + 1, Display.Height + 1);
			MessSurface = new RenderTarget2D(Engine.GraphicsDevice, Width * 16, Height * 16, false, Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
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
			
			if (loadMarked) {
				loadMarked = false;
				CreatePassable();
			}
			
			time += dt;
		}
		
		// Renders light emited by tiles
		public void RenderTileLights() {
			if (!LevelLayerDebug.TileLight) {
				return;
			}
			
			var camera = Camera.Instance;
			
			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderTop(camera);

			Graphics.Color = new Color(1f, 1f, 0f, 1f);
			
			for (int y = GetRenderBottom(camera); y >= toY; y--) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);
					var light = Light[index];

					if (NoLightNoRender && light < LightMin) {
						continue;
					}
					
					// var tile = (Tile) Tiles[index];
					var liquid = (Tile) Liquid[index];

					if (liquid == Tile.Lava) {
						Graphics.Render(Tilesets.Biome.Light[LiquidVariants[index]], new Vector2(x * 16 - 24, y * 16 - 24), 0, Vector2.Zero, new Vector2(2));
					}
				}
			}

			Graphics.Color = ColorUtils.WhiteColor;
		}

		// Renders floor layer
		public override void Render() {
			if (this != Run.Level) {
				Done = true;
				return;
			}

			if (!LevelLayerDebug.Floor) {
				return;
			}
			
			manager.Update();

			var camera = Camera.Instance;
			
			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderTop(camera);
			var active = !Engine.Instance.State.Paused;

			var shader = Shaders.Chasm;
			Shaders.Begin(shader);

			shader.Parameters["h"].SetValue(8f / Tileset.WallTopA.Texture.Height);
			var enabled = shader.Parameters["enabled"];
			enabled.SetValue(false);
							
			for (int y = GetRenderBottom(camera); y >= toY; y--) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);
					var light = Light[index];

					if (NoLightNoRender && light < LightMin) {
						continue;
					}
					
					var tile = Tiles[index];
					var t = (Tile) tile;

					if (tile > 0) {
						if (t.Matches(TileFlags.FloorLayer)) {
							if (active && CheckFlag(index, Flag.Burning) && Rnd.Chance(10)) {
								Area.Add(new FireParticle {
									Position = new Vector2(x * 16 + Rnd.Float(-2, 18), y * 16 + Rnd.Float(-2, 18)),
									XChange = 0.1f,
									Scale = 0.3f,
									Vy = 8,
									T = 0.5f,
									B = 0
								});
							}
							
							var pos = new Vector2(x * 16, y * 16);

							if (t == Tile.PistonDown) {
								RenderWall(x, y, index, tile, t, 0);
							} else if (t != Tile.Chasm && t != Tile.SpikeOffTmp && t != Tile.SensingSpikeTmp) {
								#if DEBUG
								try {
#endif
									Graphics.Render((MatrixLeak[index] ? MatrixTileset : Tileset).Tiles[tile][
#if ART_DEBUG
										0
#else
										Variants[index]
#endif
									], pos);
#if DEBUG
								} catch (Exception e) {
									var variant = Variants[index];
									Log.Error($"Variant: {variant}, {e}");
								}
#endif
							}
						}
					}
				}
			}

			Shaders.End(); 
		}

		public void RenderShadows() {
			if (Done) {
				return;
			}
			
			if (!LevelLayerDebug.Shadows) {
				return;
			}
			
			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);

			for (int y = toY; y >= GetRenderTop(camera); y--) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);
					var tl = (Tile) Tiles[index];
					var tileset = (MatrixLeak[index] ? MatrixTileset : Tileset);

					if (tl.Matches(TileFlags.WallLayer) && (IsInside(index + width))) {
						var t = (Tile) Tiles[index + width];

						if (!t.IsWall() && t != Tile.Chasm) {
							Graphics.Render(tileset.WallA[CalcWallIndex(x, y)], new Vector2(x * 16, y * 16 + 10), 0, Vector2.Zero,
								Vector2.One, SpriteEffects.FlipVertically);
						}
					}

					if (tl != Tile.Transition && (tl.IsWall() || tl == Tile.PistonDown)) {
						var v = Variants[index];
						var ar = tileset.WallAExtensions;
						
						switch (tl) {
							case Tile.WallB: {
								ar = tileset.WallBExtensions;
								break;
							}
							
							case Tile.Planks: {
								ar = Tilesets.Biome.PlanksExtensions;
								break;
							}
							
							case Tile.GrannyWall: {
								ar = Tilesets.Biome.GrannyExtensions;
								break;
							}
							
							case Tile.EvilWall: {
								ar = Tilesets.Biome.EvilExtensions;
								break;
							}
						}

						if (!BitHelper.IsBitSet(v, 1)) {
							Graphics.Render(ar[1], new Vector2(x * 16 + 16, y * 16 + 9));
						}

						if (!BitHelper.IsBitSet(v, 2)) {
							Graphics.Render(ar[2], new Vector2(x * 16, y * 16 + 16 + 8));
						}

						if (!BitHelper.IsBitSet(v, 3)) {
							Graphics.Render(ar[3], new Vector2(x * 16 - 8, y * 16 + 9));
						}
					}
					
					var l = Liquid[index];
					var lt = (Tile) l;

					if (lt.IsRock()) {
						Graphics.Render(tileset.Tiles[l][LiquidVariants[index]], new Vector2(x * 16, y * 16 + 3));
					} else if (lt == Tile.MetalBlock) {
						Graphics.Render(tileset.MetalBlockShadow, new Vector2(x * 16, y * 16 + 6), 0, Vector2.Zero,
							Vector2.One, SpriteEffects.FlipVertically);
					}
				}
			}
		}

		private bool cleared;

		public void RenderMess() {
			if (!LevelLayerDebug.Mess) {
				return;
			}
			
			var camera = Camera.Instance;
			var state = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;
			state.End();

			Engine.GraphicsDevice.SetRenderTarget(MessSurface);
			Graphics.Batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, 
				state.RasterizerState, null, Matrix.Identity);

			if (!cleared) {
				cleared = true;
				Graphics.Clear(Color.White);
				// Uncomment for a wild graphics effect
			}/* else {
				Graphics.Clear(new Color(1f, 1f, 1f, 1 / 255f));
			}*/

			Graphics.Color = ColorUtils.WhiteColor;
			
			foreach (var p in Area.Tagged[Tags.Mess]) {
				((SplashFx) p).RenderInSurface();
			}
			
			Graphics.Color = ColorUtils.WhiteColor;

			Graphics.Batch.End();
			Engine.GraphicsDevice.SetRenderTarget(state.GameTarget);
			Graphics.Batch.Begin(SpriteSortMode.Immediate, messBlend, SamplerState.PointClamp, DepthStencilState.None, 
				state.RasterizerState, null, Camera.Instance?.Matrix);
			
			var region = new TextureRegion();

			region.Texture = MessSurface;
			region.Source.X = (int) Math.Floor(camera.X);
			region.Source.Y = (int) Math.Floor(camera.Y);
			region.Source.Width = Display.Width + 1;
			region.Source.Height = Display.Height + 1;
			
			Graphics.Color = FloorColor;

			Graphics.Render(region, camera.TopLeft - new Vector2(camera.Position.X % 1, 
				                        camera.Position.Y % 1));
			Graphics.Color = ColorUtils.WhiteColor;
			
			Graphics.Batch.End();
			Engine.GraphicsDevice.SetRenderTarget(state.GameTarget);
			state.Begin();
		}
		
		public void RenderRocks() {
			if (!LevelLayerDebug.Rocks) {
				return;
			}

			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);

			for (int y = toY; y >= GetRenderTop(camera); y--) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);
					var light = Light[index];

					if (NoLightNoRender && light < LightMin) {
						continue;
					}
					
					var tile = Liquid[index];

					if (tile > 0) {
						var tt = (Tile) tile;

						if (tt.IsHalfWall()) {
							Graphics.Render(Tileset.Tiles[tile][LiquidVariants[index]], new Vector2(x * 16, y * 16));
						}
					}
				}
			}
		}
		
		public void RenderLiquids() {
			if (!LevelLayerDebug.Liquids) {
				return;
			}

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

			for (int y = toY; y >= GetRenderTop(camera); y--) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);
					var light = Light[index];

					if (NoLightNoRender && light < LightMin) {
						continue;
					}
					
					var tile = Liquid[index];

					if (tile > 0) {
						var tt = (Tile) tile;

						if (tt == Tile.Collider) {
							#if DEBUG
							if (Engine.EditingLevel) {
								enabled.SetValue(false);
								Graphics.Render(Tilesets.Biome.Collider, new Vector2(x * 16, y * 16));
								enabled.SetValue(true);
							}
							#endif
							
							continue;
						}

						if (tt.IsHalfWall()) {
							continue;
						}
						
						region.Set(Tilesets.Biome.Patterns[tile]);
						region.Source.Width = 16;
						region.Source.Height = 16;

						if (tt == Tile.Water) {
							flow.SetValue(1f);
							sy.SetValue(y % 4 * 16f / Tilesets.Biome.WaterPattern.Texture.Height);
						} else if (tt == Tile.Lava) {
							flow.SetValue(0.25f);
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

							if (t == Tile.Water || t == Tile.Lava) {
								if (t == Tile.Lava && Rnd.Chance(0.5f)) {
									var p = Particles.Wrap(Particles.Lava(), Area, pos + Rnd.Vector(0, 16));
									p.Particle.Velocity = MathUtils.CreateVector(Rnd.Float(-10, 10), -Rnd.Float(30, 45));
									p.Particle.Scale = Rnd.Float(0.3f, 0.5f);
									p.Particle.T = 0;
								}
								
								if (!paused && Get(index + width) == Tile.Chasm && Rnd.Chance(6)) {
									Area.Add(new WaterfallFx {
										Position = pos + new Vector2(Rnd.Float(16), 16),
										Lava = t == Tile.Lava
									});
								}
							}
						} else {
							enabled.SetValue(false);
							Graphics.Render(region, pos);
							enabled.SetValue(true);
						}
						
						if (tt == Tile.Water || tt == Tile.Lava) {
							flow.SetValue(0f);
						}
					}
				}
			}
			
			Shaders.End();
			RenderMess();
		}

		private TextureRegion clear;
		
		private void RenderChasms() {
			if (!LevelLayerDebug.Chasms) {
				return;
			}
			
			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);
			
			var active = !Engine.Instance.State.Paused;
			var state = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;
			state.End();

			if (clear == null) {
				clear = CommonAse.Particles.GetSlice("wall");
			}

			Engine.GraphicsDevice.SetRenderTarget(MessSurface);
			Graphics.Batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, 
				state.RasterizerState, null, Matrix.Identity);

			for (int y = GetRenderTop(camera); y < toY; y++) {
				for (int x = GetRenderLeft(camera); x < toX; x++) {
					if ((Tile) Tiles[ToIndex(x, y)] == Tile.Chasm) {
						Graphics.Render(clear, new Vector2(x * 16, y * 16));
					}
				}
			}

			Graphics.Batch.End();
			Engine.GraphicsDevice.SetRenderTarget(state.GameTarget);
			var shader = Shaders.Chasm;
			Graphics.Batch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, 
					state.RasterizerState, shader, Camera.Instance?.Matrix);

			shader.Parameters["h"].SetValue(8f / Tileset.WallTopA.Texture.Height);
			var sy = shader.Parameters["y"];
			var enabled = shader.Parameters["enabled"];
			enabled.SetValue(false);

			for (int y = GetRenderTop(camera); y < toY; y++) {
				for (int x = GetRenderLeft(camera); x < toX; x++) {
					var index = ToIndex(x, y);

					if ((Tile) Tiles[index] == Tile.Chasm) {
						var pos = new Vector2(x * 16, y * 16);

						Graphics.Render(Tilesets.Biome.ChasmPattern, pos);
						
						if (active && Rnd.Chance(0.1f)) {
							Area.Add(new ChasmFx {
								Position = pos + new Vector2(Rnd.Float(16), Rnd.Float(16))
							});
						}

						if (index >= width) {
							var tt = Get(index - width);

							if (tt != Tile.Chasm) {
								var ind = CalcWallSide(x, y);
								var tileset = (MatrixLeak[index] ? MatrixTileset : Tileset);
								TextureRegion textureRegion;
								
								switch (tt) {
									case Tile.WallA: case Tile.Piston: case Tile.PistonDown:
										textureRegion = tileset.WallA[ind];
										break;
									case Tile.Planks:
										textureRegion = Tilesets.Biome.Planks[ind];
										break;
									case Tile.EvilWall: case Tile.EvilFloor:
										textureRegion = Tilesets.Biome.EvilWall[ind];
										break;
									case Tile.GrannyWall: case Tile.GrannyFloor:
										textureRegion = Tilesets.Biome.GrannyWall[ind];
										break;
									case Tile.FloorA:
										textureRegion = tileset.FloorSidesA[ind];
										break;
									case Tile.FloorB:
										textureRegion = tileset.FloorSidesB[ind];
										break;
									case Tile.FloorC:
										textureRegion = tileset.FloorSidesC[ind];
										break;
									case Tile.FloorD:
										textureRegion = tileset.FloorSidesD[ind];
										break;

									default:
									case Tile.WallB:
										textureRegion = tileset.WallB[ind];
										break;
								}

								enabled.SetValue(true);								
								sy.SetValue((float) textureRegion.Source.Y / textureRegion.Texture.Height);
								Graphics.Render(textureRegion, pos);
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
					
						enabled.SetValue(false);
					}
				}
			}
			
			Shaders.End();
		}
		
		private void RenderSides() {
			if (!LevelLayerDebug.Sides) {
				return;
			}
			
			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);
			
			for (int y = GetRenderTop(camera); y < toY; y++) {
				for (int x = GetRenderLeft(camera); x < toX; x++) {
					var index = ToIndex(x, y);
					var tl = (Tile) Tiles[index];
					
					if (tl.Matches(TileFlags.WallLayer)) {
						if ((IsInside(index + width) && !((Tile) Tiles[index + width]).IsWall())) {
							var pos = new Vector2(x * 16, y * 16 + 8);
							var a = tl.Matches(Tile.WallA, Tile.Piston);
							
							if (tl == Tile.Crack) {
								a = (IsInside(index + 1) && Get(index + 1) == Tile.WallA) ||
								    (IsInside(index + width) && Get(index + width) == Tile.WallA);
							}

							var tileset = (MatrixLeak[index] ? MatrixTileset : Tileset);
							var ar = tileset.WallA;
							var arr = tileset.WallSidesA;

							if (!a) {
								switch (tl) {
									case Tile.Planks: {
										ar = Tilesets.Biome.Planks;
										arr = Tilesets.Biome.PlankSides;
										break;
									}

									case Tile.Crack: {
										if (!(IsInside(index + 1) && Get(index + 1) == Tile.WallA) &&
										    !(IsInside(index + width) && Get(index + width) == Tile.WallA)) {
											
											ar = tileset.WallB;
											arr = tileset.WallSidesB;
										}


										break;
									}
									
									case Tile.WallB: {
										ar = tileset.WallB;
										arr = tileset.WallSidesB;
										break;
									}
																		
									case Tile.EvilWall: {
										ar = Tilesets.Biome.EvilWall;
										arr = Tilesets.Biome.EvilWallSides;
										break;
									}

									case Tile.GrannyWall: {
										ar = Tilesets.Biome.GrannyWall;
										arr = Tilesets.Biome.GrannyWallSides;
										break;
									}
								}
							}
							
							var ind = -1;

							if (index >= Size - 1 || !((Tile) Tiles[index + 1]).Matches(Tile.Piston, Tile.WallA, Tile.WallB,
								    Tile.Planks, Tile.EvilWall, Tile.GrannyWall, Tile.Transition)) {
								ind += 1;
							}

							if (index <= 0 || !((Tile) Tiles[index - 1]).Matches(Tile.Piston, Tile.WallA, Tile.WallB, Tile.EvilWall,
								    Tile.GrannyWall, Tile.Planks, Tile.Transition)) {
								ind += 2;
							}

							if (ind != -1) {
								Graphics.Render(arr[ind], pos);
							} else {
								Graphics.Render(ar[CalcWallIndex(x, y)], pos);
							}
						}
					} else if (tl == Tile.Chasm) {
						if (IsInside(index + width) && !Get(index + width).IsWall() && Get(index + width) != Tile.Chasm) {
							Graphics.Render(Tilesets.Biome.ChasmBottom[CalcChasmIndex(x, y + 1)], new Vector2(x * 16, y * 16 + 16));
						}
								
						if (IsInside(index + 1) && !Get(index + 1).IsWall() && Get(index + 1) != Tile.Chasm) {
							Graphics.Render(Tilesets.Biome.ChasmRight[CalcChasmIndex(x + 1, y)], new Vector2(x * 16 + 16, y * 16));
						}
								
						if (index > 0 && !Get(index - 1).IsWall() && Get(index - 1) != Tile.Chasm) {
							Graphics.Render(Tilesets.Biome.ChasmLeft[CalcChasmIndex(x - 1, y)], new Vector2(x * 16 - 16, y * 16));
						}
					}
				}
			}
		}

		private void RenderShadowSurface() {
			if (!LevelLayerDebug.Shadows) {
				return;
			}
			
			if (Done) {
				return;
			}
			
			if (Engine.Instance.StateRenderer.UiTarget != null) {
				Graphics.Color = ShadowColor;

				var c = Camera.Instance;
				var z = c.Zoom;
				var n = Math.Abs(z - 1) > 0.01f;
				
				if (n) {
					c.Zoom = 1;
					c.UpdateMatrices();
				}

				Graphics.Render(Engine.Instance.StateRenderer.UiTarget,
					Camera.Instance.TopLeft - new Vector2(Camera.Instance.Position.X % 1, 
						Camera.Instance.Position.Y % 1));

				if (n) {
					c.Zoom = z;
					c.UpdateMatrices();
				}
				
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}
		
		private void RenderWall(int x, int y, int index, int tile, Tile t, int m) {
			var a = false;
			var tileset = (MatrixLeak[index] ? MatrixTileset : Tileset);
			
			if (t != Tile.Transition) {
				var region = t == Tile.Planks ? Tilesets.Biome.PlanksTop : tileset.Tiles[tile][0];
				a = t == Tile.WallA || t == Tile.Piston || t == Tile.PistonDown;
				var ab = a || t == Tile.GrannyWall || t == Tile.EvilWall;
				var effect = Graphics.ParseEffect(x % 2 == 0, y % 2 == 0);

				if (ab) {
					var v = WallDecor[index];

					if (!t.Matches(Tile.Piston, Tile.PistonDown) && v > 0) {
						region = Tileset.WallVariants[v - 1];
					}
				} else if (t == Tile.Crack) {
					ab = (IsInside(index + 1) && Get(index + 1) == Tile.WallA) ||
					     (IsInside(index + width) && Get(index + width) == Tile.WallA);
					region = ab
						? tileset.WallTopA
						: tileset.WallTopB;
				} else {
					effect = SpriteEffects.None;
				}

				Graphics.Render(region, new Vector2(x * 16, y * 16 - 8), 0, Vector2.Zero, Vector2.One, effect);
			}
			
			if (t.IsWall() || t == Tile.PistonDown) {
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

						var ar = tileset.WallTopsA;

						if (!a) {
							switch (t) {
								case Tile.Transition: {
									ar = tileset.WallTopsTransition;
									break;
								}
								
								case Tile.WallB: {
									ar = tileset.WallTopsB;
									break;
								}
								
								case Tile.Crack: {
									if (!(IsInside(index + 1) && Get(index + 1) == Tile.WallA) && !(IsInside(index + width) && Get(index + width) == Tile.WallA)) {
										ar = tileset.WallTopsB;
									}

									break;
								}
								
								case Tile.Planks: {
									ar = Tilesets.Biome.PlankTops;
									break;
								}
								
								case Tile.GrannyWall: {
									ar = Tilesets.Biome.GrannyWallTops;
									break;
								}
								
								case Tile.EvilWall: {
									ar = Tilesets.Biome.EvilWallTops;
									break;
								}
							}
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

							if (lv != 15) {
								var vl = Tileset.WallMapExtra[lv];

								if (vl != -1) {
									var i = ToIndex(x + (xx == 0 ? -1 : 1), y + yy - 1);
									var light = DrawLight ? (i < 0 || i >= Light.Length ? 1 : Light[i]) : 1;

									if (light > LightMin) {
										Graphics.Color.A = (byte) (light * 255);
										var ind = vl + 12 * CalcWallTopIndex(x, y);
										
										Graphics.Render(ar[ind],
											new Vector2(x * 16 + xx * 8, y * 16 + yy * 8 - m));

										Graphics.Color.A = 255;
									}
								}
							}
						} else {
							var vl = Tileset.WallMap[lv];
							
							if (vl != -1) {
								var i = ToIndex(x + (xx == 0 ? -1 : 1), y + yy - 1);
								var light = DrawLight ? (i < 0 || i >= Light.Length ? 1 : Light[i]) : 1;

								if (light > LightMin) {
									Graphics.Color.A = (byte) (light * 255);

									var ind = vl + 12 * CalcWallTopIndex(x, y);
									
									Graphics.Render(ar[ind],
										new Vector2(x * 16 + xx * 8, y * 16 + yy * 8 - m));

									Graphics.Color.A = 255;
								}
							}
						}
					}
				}

				if (t != Tile.Transition) {
					var ar = tileset.WallAExtensions;

					switch (t) {
						case Tile.WallB: {
							ar = tileset.WallBExtensions;
							break;
						}
							
						case Tile.Planks: {
							ar = Tilesets.Biome.PlanksExtensions;
							break;
						}
							
						case Tile.GrannyWall: {
							ar = Tilesets.Biome.GrannyExtensions;
							break;
						}
							
						case Tile.EvilWall: {
							ar = Tilesets.Biome.EvilExtensions;
							break;
						}
					}
					
					if (!BitHelper.IsBitSet(v, 0)) {
						Graphics.Render(ar[0], new Vector2(x * 16, y * 16 - m - 8));
					}

					if (!BitHelper.IsBitSet(v, 1)) {
						Graphics.Render(ar[1], new Vector2(x * 16 + 16, y * 16 - m));
					}

					if (!BitHelper.IsBitSet(v, 2)) {
						Graphics.Render(ar[2], new Vector2(x * 16, y * 16 - m + 16));
					}

					if (!BitHelper.IsBitSet(v, 3)) {
						Graphics.Render(ar[3], new Vector2(x * 16 - 8, y * 16 - m));
					}
				}
			}
		}
		
		public void RenderWalls() {
			if (!LevelLayerDebug.Walls) {
				return;
			}
			
			var camera = Camera.Instance;
			var state = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;
			state.End();
			
			var effect = state.SurfaceEffect;
			state.SurfaceEffect = null;

			Engine.GraphicsDevice.SetRenderTarget(WallSurface);

			Graphics.Batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, 
				state.RasterizerState, null, Camera.Instance?.Matrix);
			Graphics.Clear(Color.TransparentBlack);

			Graphics.Color = ColorUtils.WhiteColor;
			
			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);

			for (int y = GetRenderTop(camera); y <= toY; y++) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);

					var tile = Tiles[index];
					var t = (Tile) tile;

					if (tile > 0 && t.Matches(TileFlags.WallLayer)) {
						RenderWall(x, y, index, tile, t, 8);
					}
				}
			}

			Graphics.Batch.End();
			RenderBlood();
			Graphics.Batch.Begin(SpriteSortMode.Immediate, blend, SamplerState.PointClamp, DepthStencilState.None, 
				state.RasterizerState, null, Camera.Instance?.Matrix);
			
			foreach (var p in Area.Tagged[Tags.Player]) {
				((Player) p).RenderOutline();
			}
			
			Graphics.Batch.End();
			Engine.GraphicsDevice.SetRenderTarget(state.GameTarget);
			
			var c = Camera.Instance;
			var z = c.Zoom;
			var n = Math.Abs(z - 1) > 0.01f;
			
			if (n) {
				c.Zoom = 1;
				c.UpdateMatrices();
			}

			Graphics.Batch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
				state.RasterizerState, null, Camera.Instance?.Matrix);
			
			Graphics.Render(WallSurface, Camera.Instance.TopLeft - new Vector2(Camera.Instance.Position.X % 1, 
				                             Camera.Instance.Position.Y % 1));
			
			Graphics.Batch.End();

			if (n) {
				c.Zoom = z;
				c.UpdateMatrices();
			}
			
			Engine.GraphicsDevice.SetRenderTarget(state.GameTarget);
			state.SurfaceEffect = effect;
			state.Begin();
			
			if (!RenderPassable) {
				return;
			}
			
			var color = new Color(1f, 1f, 1f, 0.5f);

			for (int y = GetRenderTop(camera); y <= toY; y++) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					if (Passable[ToIndex(x, y)]) {
						Graphics.Batch.DrawRectangle(new RectangleF(x * 16 + 1, y * 16 + 1, 14, 14), color);
					}
				}
			}
		}
		
		public void RenderBlood() {
			if (!LevelLayerDebug.Blood) {
				return;
			}
			
			if (MessSurface == null) {
				return;
			}
			
			var camera = Camera.Instance;
			var state = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;

			Graphics.Batch.Begin(SpriteSortMode.Immediate, messBlend, SamplerState.PointClamp, DepthStencilState.None, 
				state.RasterizerState, null, Camera.Instance?.Matrix);
			
			var region = new TextureRegion();

			region.Texture = MessSurface;
			region.Source.X = (int) Math.Floor(camera.X);
			region.Source.Y = (int) Math.Floor(camera.Y) + 8;
			region.Source.Width = Display.Width + 1;
			region.Source.Height = Display.Height + 1;
			
			Graphics.Render(region, camera.TopLeft - new Vector2(camera.Position.X % 1, 
				                        camera.Position.Y % 1));
			
			Graphics.Batch.End();
		}
		
		public void RenderLight() {
			if (!DrawLight || !LevelLayerDebug.TileLight) {
				return;
			}
			
			var camera = Camera.Instance;

			// Cache the condition
			var toX = GetRenderRight(camera);
			var toY = GetRenderBottom(camera);

			var dt = Engine.Delta * 10f;
			var region = Tileset.WallTopA;
			
			for (int y = GetRenderTop(camera); y <= toY; y++) {
				for (int x = GetRenderLeft(camera); x <= toX; x++) {
					var index = ToIndex(x, y);
					var light = Light[index];

					if (Explored[index] && light < LightMax) {
						Light[index] = light = Math.Min(1, light + dt);
					}

					if (light < LightMax) {
						Graphics.Color.A = (byte) (255 - light * 255);
						Graphics.Render(region, new Vector2(x * 16, y * 16));
					}

					if (Tiles[index] == (byte) Tile.Crack) {
						Graphics.Color.A = 255;
						Graphics.Render(Tileset.WallCrackA, new Vector2(x * 16, y * 16 - 8));
					}
				}
			}
			
			Graphics.Color.A = 255;
		}
		
		private byte CalcWallIndex(int x, int y) {
			#if ART_DEBUG
				return 0;
			#else
				return (byte) (((int) Math.Round(x * 3.5f + y * 2.74f)) % 12);
			#endif
		}

		private byte CalcWallSide(int x, int y) {
#if ART_DEBUG
				return 0;
#else
			return (byte) (((int) Math.Round(x * 3.5f + y * 2.74f)) % 4);
#endif
		}

		private byte CalcChasmIndex(int x, int y) {
#if ART_DEBUG
				return 0;
#else
			return (byte) (((int) Math.Round(x * 7.417f + y * 2.12f)) % 3);
#endif
		}
		
		private byte CalcWallTopIndex(int x, int y) {
#if ART_DEBUG
				return 0;
#else
			return (byte) (((int) Math.Round(x * 16.217f + y * 8.12f)) % 3);
#endif
		}

		public virtual Tile GetFilling() {
			return Biome.GetFilling();
		}

		public virtual int GetPadding() {
			return 1;
		}

		private void ResizeArray<T>(ref T[] array, int w, int h, int newSize, T val) {
			var newArray = new T[newSize];

			for (var y = 0; y < h; y++) {
				for (var x = 0; x < w; x++) {
					newArray[x + y * w] = x >= Width || y >= Height ? val : array[x + y * Width];
				}
			}

			array = newArray;
		}

		public void Resize(int w, int h) {
			var size = w * h;
			
			ResizeArray(ref Tiles, w, h, size, (byte) Tile.FloorA);
			ResizeArray(ref Liquid, w, h, size, (byte) 0);
			ResizeArray(ref Variants, w, h, size, (byte) 0);
			ResizeArray(ref LiquidVariants, w, h, size, (byte) 0);
			ResizeArray(ref Flags, w, h, size, (byte) 0);
			ResizeArray(ref Explored, w, h, size, false);
			ResizeArray(ref Passable, w, h, size, false);
			ResizeArray(ref MatrixLeak, w, h, size, false);
			ResizeArray(ref Light, w, h, size, 0);

			Width = w;
			Height = h;
			Size = size;
			cleared = false;
			
			PathFinder.SetMapSize(Width, Height);

			RefreshSurfaces();
		}

		public void RefreshSurfaces() {
			WallSurface.Dispose();
			MessSurface.Dispose();
			cleared = false;

			WallSurface = new RenderTarget2D(Engine.GraphicsDevice, Display.Width + 1, Display.Height + 1);

			MessSurface = new RenderTarget2D(Engine.GraphicsDevice, Width * 16, Height * 16, false,
				Engine.Graphics.PreferredBackBufferFormat, DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
		}

		public virtual string GetMusic() {
			return Biome.GetMusic();
		}

		public static string GetDepthString(bool eng = false) {
			if (Run.Depth < 1) {
				return Locale.Get(Run.Level.Biome.Id, eng);
			}

			var s = $"{Locale.Get(Run.Level.Biome.Id, eng)} {MathUtils.ToRoman((Run.Depth - 1) % 2 + 1)}";

			if (Run.Loop > 0) {
				s = $"L{Run.Loop} {s}";
			}
			
			return s;
		}
		 
		public void Break(float x, float y) {
			BSet((int) Math.Floor(x / 16), (int) Math.Floor(y / 16));
			
			BSet((int) Math.Floor(x / 16 - 0.5f), (int) Math.Floor(y / 16));
			BSet((int) Math.Floor(x / 16 + 0.5f), (int) Math.Floor(y / 16));
			BSet((int) Math.Floor(x / 16), (int) Math.Floor(y / 16 - 0.5f));
			BSet((int) Math.Floor(x / 16), (int) Math.Floor(y / 16 + 0.5f));
		}

		private void BSet(int tx, int ty) {
			if (!IsInside(tx, ty)) {
				return;
			}

			var index = ToIndex(tx, ty);
			var tile = (Tile) Tiles[index];

			if (tile != Tile.Planks && (!(Biome is IceBiome) || tile != Tile.WallA)) {
				return;
			}

			Set(index, Tile.FloorA);							
			Set(index, Tile.Ember);

			UpdateTile(tx, ty);
			
			ReCreateBodyChunk(tx, ty);
			Animate(Area, tx, ty);
		}

		public static void Animate(Area area, int x, int y) {
			if (!Camera.Instance.Overlaps(new Rectangle(x * 16, y * 16, 16, 16))) {
				return;
			}

			area.Add(new TileFx {
				X = x * 16,
				Y = y * 16 - 8
			});
			
			for (var i = 0; i < 3; i++) {
				var part = new ParticleEntity(Particles.Dust());
						
				part.Position = new Vector2(x * 16 + 8, y * 16 + 8);
				area.Add(part);
			}
			
			for (var i = 0; i < 3; i++) {
				var part = new ParticleEntity(Particles.Plank());
						
				part.Position = new Vector2(x * 16 + 8, y * 16);
				part.Particle.Scale = Rnd.Float(0.4f, 0.8f);
				
				area.Add(part);
			}

			Engine.Instance.Freeze = 0.5f;
			Camera.Instance.Shake(2);
			
			AudioEmitterComponent.Dummy(area, new Vector2(x, y) * 16).EmitRandomizedPrefixed("level_chair_break", 2, 0.75f);
		}

		public void ReTileAndCreateBodyChunks(int x, int y, int w, int h) {
			UpdateTile(x, y);
			ReCreateBodyChunk(x, y);
			
			for (var yy = y - 1; yy < y + h + 1; yy++) {
				for (var xx = x - 1; xx < x + w + 1; xx++) {
					if (yy != y || xx != x) {
						UpdateTile(xx, yy);
						ReCreateBodyChunk(xx, yy);
					}
				}
			}
		}

		public override void RenderImDebug() {
			base.RenderImDebug();
			ImGui.Text($"Size: {width}x{height} = {Size} (real {width * height})");
			ImGui.Text($"Variant: {Variant.GetType().Name}");
			ImGui.Text($"Tiles: {Tiles.Length}");
			ImGui.Text($"Liquid: {Liquid.Length}");
			ImGui.Text($"Variants: {Variants.Length}");
			ImGui.Text($"LiquidVariants: {LiquidVariants.Length}");
			ImGui.Text($"WallDecor: {WallDecor.Length}");
		}
	}
}