using BurningKnight.core.entity.level.rooms;

namespace BurningKnight.core.entity.level {
	public abstract class Level : SaveableEntity {
		static Level() {
			MaskShader = new ShaderProgram(Gdx.Files.Internal("shaders/default.vert").ReadString(), Gdx.Files.Internal("shaders/mask.frag").ReadString());

			if (!MaskShader.IsCompiled()) {
				throw new GdxRuntimeException("Couldn't compile shader: " + MaskShader.GetLog());
			} 
		}

		static Level() {
			Shader = new ShaderProgram(Gdx.Files.Internal("shaders/default.vert").ReadString(), Gdx.Files.Internal("shaders/fadeout.frag").ReadString());

			if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());

		}

		public static bool RENDER_ROOM_DEBUG = false;
		public static bool RENDER_PASSABLE = false;
		public static bool SHADOWS = true;
		public static Color[] Colors = { Color.ValueOf("#0e071b"), Color.ValueOf("#391f21"), Color.ValueOf("#5d2c28"), Color.ValueOf("#1a1932"), Color.ValueOf("#272727"), Color.ValueOf("#1a1932"), Color.ValueOf("#3b1443"), Color.ValueOf("#92a1b9") };
		public Room Entrance;
		public Room Exit;
		public const Vector2[] NEIGHBOURS8V = { new Vector2(-1, -1), new Vector2(0, -1), new Vector2(1, -1), new Vector2(-1, 0), new Vector2(1, 0), new Vector2(-1, 1), new Vector2(0, 1), new Vector2(1, 1) };
		public static bool GENERATED = false;
		public static string[] COMPASS = { " NESW", " ESW", " NSW", " SW", " NEW", " EW", " NW", " W", " NES", " ES", " NS", " S", " NE", " E", " N", "" };
		public const Rectangle[] COLLISIONS = { new Rectangle(4, 4, 8, 8), new Rectangle(4, 4, 8, 12), new Rectangle(4, 4, 12, 8), new Rectangle(4, 4, 12, 12), new Rectangle(4, 0, 8, 12), new Rectangle(4, 0, 8, 16), new Rectangle(0, 0, 12, 16), new Rectangle(4, 0, 12, 16), new Rectangle(0, 4, 12, 8), new Rectangle(0, 4, 12, 12), new Rectangle(0, 4, 16, 8), new Rectangle(0, 4, 16, 12), new Rectangle(0, 0, 12, 12), new Rectangle(0, 0, 12, 16), new Rectangle(0, 0, 16, 12), new Rectangle(0, 0, 16, 16) };
		private static int Width = 36;
		private static int Height = 36;
		private static int Size = GetWidth() * GetHeight();
		public byte[] Data;
		public byte[] LiquidData;
		protected byte[] Variants;
		public byte[] LiquidVariants;
		protected byte[] Walls;
		protected float[] Light;
		protected byte[] WallDecor;
		public bool[] Passable;
		protected bool[] Low;
		protected bool[] Free;
		protected byte[] Decor;
		public bool[] Explored;
		public int[] Info;
		protected Body Body;
		protected List<Room> Rooms;
		public List<Item> ItemsToSpawn = new List<>();
		private static string[] Letters = { "I", "II", "III", "IV", "V", "VI" };
		public bool AddLight;
		public int Uid = 0;
		private float LastUpdate;
		private int UpdateId;
		private const float UPDATE_DELAY = 1f;
		private float LastFlame;
		public static ShaderProgram MaskShader;
		public static ShaderProgram Shader;
		private TextureRegion BloodRegion;
		private Body Chasms;

		public Void ExploreAll() {
			for (int I = 0; I < GetSize(); I++) {
				if (Data[I] > -1) {
					Explored[I] = true;
				} 
			}
		}

		public Void ExploreRandom() {
			for (int I = 0; I < GetSize(); I++) {
				if ((Data[I] > -1) && Random.Chance(50)) {
					Explored[I] = true;
				} 
			}
		}

		public Void SetPassable(int X, int Y, bool V) {
			this.Passable[ToIndex(X, Y)] = V;
		}

		public static int GetWidth() {
			return Width;
		}

		public static Void SetWidth(int W) {
			Level.Width = W;
			Level.Size = Level.Width * Level.Height;
		}

		public static Void SetSize(int Width, int Height) {
			Level.Width = Width;
			Level.Height = Height;
			Level.Size = Width * (Height + 1);
		}

		public Void GenerateDecor() {
			Decor = new byte[GetSize()];
			WallDecor = new byte[GetSize()];
		}

		public static int GetHeight() {
			return Height;
		}

		public static Void SetHeight(int H) {
			Level.Height = H;
			Level.Size = Level.Width * Level.Height;
		}

		public static int GetSize() {
			return Size;
		}

		public static int ToIndex(int X, int Y) {
			return X + Y * GetWidth();
		}

		public static int ToX(int I) {
			return I % Width;
		}

		public static int ToY(int I) {
			return (int) Math.Floor(I / Width);
		}

		public static RegularLevel ForDepth(int Depth) {
			string Seed = Random.GetSeed();

			switch (Seed) {
				case "ICE": {
					return new IceLevel();
				}

				case "CASTLE": {
					return new HallLevel();
				}

				case "FOREST": {
					return new ForestLevel();
				}

				case "LIBRARY": {
					return new LibraryLevel();
				}

				case "DESERT": {
					return new DesertLevel();
				}

				case "BLOOD": {
					return new BloodLevel();
				}

				case "TECH": {
					return new TechLevel();
				}
			}

			if (Depth < 2) {
				return new HallLevel();
			} else if (Depth < 3) {
				return new DesertLevel();
			} else if (Depth < 4) {
				return new ForestLevel();
			} else if (Depth < 5) {
				return new LibraryLevel();
			} else if (Depth < 6) {
				return new IceLevel();
			} else if (Depth < 7) {
				return new TechLevel();
			} else if (Depth < 8) {
				return new BloodLevel();
			} 

			return new BossLevel();
		}

		public string GetDepthAsCoolNum() {
			return Letters[0];
		}

		public string FormatDepth() {
			if (Dungeon.Depth == -3) {
				return Locale.Get("introduction");
			} 

			if (Dungeon.Depth == -2) {
				return Locale.Get("traders_asylum");
			} else if (Dungeon.Depth == -1) {
				return Locale.Get("castle");
			} else if (Dungeon.Depth == 0) {
				return Locale.Get("beginning");
			} else {
				return GetName();
			}

		}

		public Void InitLight() {
			this.Light = new float[GetSize()];
			Arrays.Fill(this.Light, Dungeon.Level.AddLight && false ? 1f : 0f);
		}

		public Void Fill() {
			this.Data = new byte[GetSize()];
			this.Info = new int[GetSize()];
			this.LiquidData = new byte[GetSize()];
			this.Explored = new bool[GetSize()];
			this.InitLight();
			byte Tile = Terrain.WALL;

			for (int I = 0; I < GetSize(); I++) {
				this.Data[I] = Tile;
			}
		}

		public Void LoadPassable() {
			this.Passable = new bool[GetSize()];
			this.Low = new bool[GetSize()];
			this.Variants = new byte[GetSize()];
			this.LiquidVariants = new byte[GetSize()];
			this.Walls = new byte[GetSize()];

			for (int I = 0; I < GetSize(); I++) {
				this.Passable[I] = this.CheckFor(I, Terrain.PASSABLE);
				this.Low[I] = !this.CheckFor(I, Terrain.HIGH);
			}

			for (int Y = 0; Y < Height; Y++) {
				for (int X = 0; X < Width; X++) {
					this.Tile(X, Y);
				}
			}
		}

		public Void UpdateTile(int X, int Y) {
			for (int Yy = Y - 1; Yy <= Y + 2; Yy++) {
				for (int Xx = X - 1; Xx <= X + 2; Xx++) {
					this.Tile(Xx, Yy, false);
				}
			}

			int I = ToIndex(X, Y);
			this.Passable[I] = this.CheckFor(I, Terrain.PASSABLE);
		}

		public Void Tile(int X, int Y) {
			Tile(X, Y, true);
		}

		public Void Tile(int X, int Y, bool Walls) {
			byte Tile = this.Get(X, Y);

			if (Tile == Terrain.CHASM) {
				this.TileUp(X, Y, Tile, false);
			} else if (Walls && (Tile == Terrain.WALL || Tile == Terrain.CRACK)) {
				this.TileUp(X, Y, Tile, false);
				this.WallDecor[ToIndex(X, Y)] = (byte) (Random.NewInt(3));
			} else if (Tile == Terrain.TABLE) {
				this.TileUp(X, Y, Tile, false);
			} else if (Tile == Terrain.GRASS || Tile == Terrain.HIGH_GRASS || Tile == Terrain.HIGH_DRY_GRASS || Tile == Terrain.DRY_GRASS) {
				this.TileUp(X, Y, Tile, false);
			} else if (Tile == Terrain.ICE) {
				this.TileUp(X, Y, Tile, false);
			} else if (Tile == Terrain.OBSIDIAN) {
				this.TileUp(X, Y, Tile, false);
			} else if (Tile == Terrain.FLOOR_A || Tile == Terrain.FLOOR_B || Tile == Terrain.FLOOR_C || Tile == Terrain.FLOOR_D) {
				this.MakeFloor(X, Y, Tile);
			} else if (Tile == Terrain.DISCO) {
				this.TileUp(X, Y, Tile, false);
			} 

			int I = ToIndex(X, Y);
			byte T = this.LiquidData[I];

			if (MatchesFlag(T, Terrain.LIQUID_LAYER)) {
				this.TileUpLiquid(X, Y, T, false);
			} 

			if (!Walls) {
				return;
			} 

			byte Count = 0;

			if (!this.ShouldTile(X, Y + 1, Terrain.WALL, false)) {
				Count += 1;
			} 

			if (!this.ShouldTile(X + 1, Y, Terrain.WALL, false)) {
				Count += 2;
			} 

			if (!this.ShouldTile(X, Y - 1, Terrain.WALL, false)) {
				Count += 4;
			} 

			if (!this.ShouldTile(X - 1, Y, Terrain.WALL, false)) {
				Count += 8;
			} 

			if (!this.ShouldTile(X + 1, Y + 1, Terrain.WALL, false)) {
				Count += 16;
			} 

			if (!this.ShouldTile(X + 1, Y - 1, Terrain.WALL, false)) {
				Count += 32;
			} 

			if (!this.ShouldTile(X - 1, Y - 1, Terrain.WALL, false)) {
				Count += 64;
			} 

			if (!this.ShouldTile(X - 1, Y + 1, Terrain.WALL, false)) {
				Count += 128;
			} 

			this.Walls[ToIndex(X, Y)] = Count;
		}

		public Void TileRegion(int X, int Y) {
			for (int Yy = Y - 1; Yy <= Y + 1; Yy++) {
				for (int Xx = X - 1; Xx <= X + 1; Xx++) {
					this.Tile(Xx, Yy);
				}
			}
		}

		private Void MakeFloor(int X, int Y, int Tile) {
			int I = ToIndex(X, Y);

			if (this.Variants[I] != 0) {
				return;
			} 

			byte Var = (byte) Random.NewInt(0, 11);

			if (Var == 9 || Var == 10) {
				for (int Yy = Y; Yy < Y + 2; Yy++) {
					for (int Xx = X; Xx < X + 2; Xx++) {
						if (this.Get(Xx, Yy) != Tile || this.Variants[ToIndex(Xx, Yy)] != 0) {
							Var = (byte) Random.NewInt(0, 9);

							break;
						} 
					}
				}
			} 

			if (Var == 9) {
				this.Variants[ToIndex(X, Y)] = 10;
				this.Variants[ToIndex(X + 1, Y)] = 11;
				this.Variants[ToIndex(X, Y + 1)] = 8;
				this.Variants[ToIndex(X + 1, Y + 1)] = 9;
			} else if (Var == 10) {
				this.Variants[ToIndex(X, Y)] = 14;
				this.Variants[ToIndex(X + 1, Y)] = 15;
				this.Variants[ToIndex(X, Y + 1)] = 12;
				this.Variants[ToIndex(X + 1, Y + 1)] = 13;
			} else {
				this.Variants[I] = (byte) MathUtils.Clamp(1, 7, Var);
			}

		}

		private Void TileUp(int X, int Y, int Tile, bool Flag) {
			byte Count = 0;

			if (this.ShouldTile(X, Y + 1, Tile, Flag)) {
				Count += 1;
			} 

			if (this.ShouldTile(X + 1, Y, Tile, Flag)) {
				Count += 2;
			} 

			if (this.ShouldTile(X, Y - 1, Tile, Flag)) {
				Count += 4;
			} 

			if (this.ShouldTile(X - 1, Y, Tile, Flag)) {
				Count += 8;
			} 

			this.Variants[ToIndex(X, Y)] = Count;
		}

		private Void TileUpLiquid(int X, int Y, int Tile, bool Flag) {
			byte Count = 0;

			if (this.ShouldTileLiquid(X, Y + 1, Tile, Flag)) {
				Count += 1;
			} 

			if (this.ShouldTileLiquid(X + 1, Y, Tile, Flag)) {
				Count += 2;
			} 

			if (this.ShouldTileLiquid(X, Y - 1, Tile, Flag)) {
				Count += 4;
			} 

			if (this.ShouldTileLiquid(X - 1, Y, Tile, Flag)) {
				Count += 8;
			} 

			this.LiquidVariants[ToIndex(X, Y)] = Count;
		}

		private bool ShouldTile(int X, int Y, int Tile, bool Flag) {
			if (!this.IsValid(X, Y)) {
				return false;
			} 

			byte T = this.Get(X, Y);

			if (Flag) {
				return this.CheckFor(X, Y, Tile) || T == Terrain.WALL || T == Terrain.CRACK;
			} else {
				return T == Tile || T == Terrain.WALL || T == Terrain.CRACK;
			}

		}

		private bool ShouldTileLiquid(int X, int Y, int Tile, bool Flag) {
			if (!this.IsValid(X, Y)) {
				return false;
			} 

			byte T = this.Get(X, Y);

			if (T == Terrain.CHASM) {
				return true;
			} 

			if (Flag) {
				return this.CheckFor(X, Y, Tile) || T == Terrain.WALL || T == Terrain.CRACK;
			} else {
				byte Tt = this.LiquidData[ToIndex(X, Y)];

				if ((Tile == Terrain.LAVA || Tt == Terrain.LAVA) && (Tile == Terrain.OBSIDIAN || Tt == Terrain.OBSIDIAN)) {
					return true;
				} 

				if ((Tile == Terrain.GRASS || Tile == Terrain.HIGH_GRASS || Tile == Terrain.HIGH_DRY_GRASS || Tile == Terrain.DRY_GRASS) && (Tt == Terrain.GRASS || Tt == Terrain.HIGH_GRASS || Tt == Terrain.HIGH_DRY_GRASS || Tt == Terrain.DRY_GRASS)) {
					return true;
				} 

				if ((Tile == Terrain.WATER || Tile == Terrain.VENOM || Tile == Terrain.ICE) && (Tt == Terrain.WATER || Tt == Terrain.VENOM || Tt == Terrain.ICE)) {
					return true;
				} 

				return Tt == Tile || T == Terrain.WALL || T == Terrain.CRACK;
			}

		}

		public bool GetPassable() {
			return this.Passable;
		}

		public override Void Init() {
			AddLight = Dungeon.Depth == -3 || Dungeon.Depth == -1;
			Dungeon.Level = this;
			this.AlwaysRender = true;
			this.AlwaysActive = true;
			this.Depth = -10;
			SolidLevel L = new SolidLevel();
			L.SetLevel(this);
			Dungeon.Area.Add(L);
			LightLevel Ll = new LightLevel();
			Ll.SetLevel(this);
			Dungeon.Area.Add(Ll);
			WallLevel Lll = new WallLevel();
			Lll.SetLevel(this);
			Dungeon.Area.Add(Lll);
			LiquidLevel Llll = new LiquidLevel();
			Llll.SetLevel(this);
			Dungeon.Area.Add(Llll);
			Dungeon.Area.Add(new SignsLevel());
			float V = this is LibraryLevel ? 0f : (this is IceLevel || this is CreepLevel || this is BossLevel ? 0.6f : 0.3f);
			World.Lights.SetAmbientLight(V, V, V, 1f);
		}

		public string GetName() {
			return "";
		}

		public Void RenderLight() {
			if (Dungeon.Level != this) {
				return;
			} 

			OrthographicCamera Camera = Camera.Game;
			Graphics.Batch.End();
			Graphics.Batch.SetProjectionMatrix(Camera.Game.Combined);
			Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
			float Zoom = Camera.Zoom;
			float Cx = Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom;
			float Cy = Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom;
			int Sx = (int) (Math.Floor(Cx / 16) - 1);
			int Sy = (int) (Math.Floor(Cy / 16) - 1);
			int Fx = (int) (Math.Ceil((Cx + Display.GAME_WIDTH * Zoom) / 16) + 1);
			int Fy = (int) (Math.Ceil((Cy + Display.GAME_HEIGHT * Zoom) / 16) + 1);
			float Dt = Gdx.Graphics.GetDeltaTime() * Dungeon.Speed;
			Color Color = Colors[Dungeon.Level.Uid];
			float Sp = Dt * 0.3f;

			for (int I = 0; I < GetSize(); I++) {
				this.Light[I] = MathUtils.Clamp(AddLight ? 1f : (this.Explored[I] ? 0.5f : 0f), 1f, this.Light[I] - Sp);
			}

			for (int Y = Math.Max(0, Sy); Y < Math.Min(Fy, GetHeight()); Y++) {
				for (int X = Math.Max(0, Sx); X < Math.Min(Fx, GetWidth()); X++) {
					int I = X + Y * GetWidth();

					if (I >= Data.Length) {
						continue;
					} 

					float V = this.Light[I];

					if (V == 1) {
						continue;
					} 

					Graphics.Shape.SetColor(Color.R, Color.G, Color.B, 1f - V);
					Graphics.Shape.Rect(X * 16, Y * 16 - 8, 16, 16);
				}
			}

			Graphics.Shape.End();
			Gdx.Gl.GlDisable(GL20.GL_BLEND);
			Graphics.Batch.SetColor(1, 1, 1, 1);

			if (Dungeon.Depth > -3 && Settings.Quality > 0) {
				Graphics.Surface.End();
				Graphics.Batch.SetShader(null);
				Graphics.Batch.Begin();
				World.Lights.SetCombinedMatrix(Camera.Game.Combined);
				World.Lights.Update();
				World.Lights.Render();
				Graphics.Batch.End();
				Graphics.Surface.Begin();
				int Src = Graphics.Batch.GetBlendSrcFunc();
				int Dst = Graphics.Batch.GetBlendDstFunc();
				Graphics.Batch.SetBlendFunction(GL20.GL_DST_COLOR, GL20.GL_ZERO);
				Graphics.Batch.Begin();
				Texture Texture = World.Lights.GetLightMapTexture();
				Graphics.Batch.Draw(Texture, Camera.Game.Position.X - Display.GAME_WIDTH / 2, Camera.Game.Position.Y + Display.GAME_HEIGHT / 2, Display.GAME_WIDTH, -Display.GAME_HEIGHT);
				Graphics.Batch.Flush();
				Graphics.Batch.End();
				Graphics.Batch.Begin();
				Graphics.Batch.SetBlendFunction(Src, Dst);
			} else {
				Graphics.Batch.Begin();
			}

		}

		private bool IsBitSet(short Data, int Bit) {
			return (Data & (1 << Bit)) != 0;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;

			if (this != Dungeon.Level) {
				Log.Error("Extra level!");
				SetDone(true);
			} 

			if (Dungeon.Depth > -1 || Dungeon.Depth == -3) {
				this.LastUpdate += Dt;
				this.LastFlame += Dt;

				if (this.LastFlame >= 0.1f) {
					this.LastFlame = 0;

					foreach (Room Room in this.Rooms) {
						for (int Y = Room.Top; Y < Room.Bottom; Y++) {
							for (int X = Room.Left; X < Room.Right; X++) {
								int I = ToIndex(X, Y);
								int Info = this.Info[I];

								if (BitHelper.IsBitSet(Info, 17)) {
									continue;
								} 

								int Spread = BitHelper.GetNumber(Info, 10, 3);

								if (Spread > 0) {
									int Step = BitHelper.GetNumber(Info, 6, 4);

									if (Step >= 15) {
										Info = BitHelper.PutNumber(Info, 10, 3, GetOverlayType(this.LiquidData[I]));
										Info = BitHelper.SetBit(Info, 17, true);
										this.Info[I] = Info;
										this.LiquidData[I] = (byte) FromOverlay(Spread);
										byte T = this.LiquidData[I];

										if (T == Terrain.ICE) {
											foreach (int J in PathFinder.NEIGHBOURS4) {
												this.Freeze(I + J);
											}
										} else if (T == Terrain.VENOM) {
											foreach (int J in PathFinder.NEIGHBOURS4) {
												this.Venom(ToX(I + J), ToY(I + J));
											}
										} 

										for (int Yy = Y - 1; Yy < Y + 2; Yy++) {
											for (int Xx = X - 1; Xx < X + 2; Xx++) {
												this.UpdateNeighbourMask(Xx, Yy);
											}
										}
									} else {
										if (Spread == 1) {
											foreach (Mob Mob in Mob.All) {
												if (Mob.IsDead() || Mob.IsFlying()) {
													continue;
												} 

												if (CollisionHelper.Check(Mob.Hx + Mob.X, Mob.Hy + Mob.Y, Mob.Hw, Mob.Hh / 3, X * 16, Y * 16 - 8, 16, 16)) {
													Mob.AddBuff(new FrozenBuff());
												} 
											}

											Player Mob = Player.Instance;

											if (Mob.IsDead() || Mob.IsFlying()) {
												return;
											} 

											if (CollisionHelper.Check(Mob.Hx + Mob.X, Mob.Hy + Mob.Y, Mob.Hw, Mob.Hh / 3, X * 16, Y * 16 - 8, 16, 16)) {
												Mob.AddBuff(new FrozenBuff());
											} 
										} 

										this.Info[I] = BitHelper.PutNumber(Info, 6, 4, Step + 1);
									}

								} 
							}
						}
					}
				} 

				if (this.LastUpdate < UPDATE_DELAY) {
					DoEffects();
				} else {
					while (this.LastUpdate >= UPDATE_DELAY) {
						this.LastUpdate = Math.Max(0, this.LastUpdate - UPDATE_DELAY);
						this.DoLogic();
						this.UpdateId += 1;
					}
				}

			} 
		}

		private int GetOverlayType(byte Liquid) {
			if (Liquid == Terrain.ICE) {
				return 1;
			} 

			if (Liquid == Terrain.WATER) {
				return 2;
			} else if (Liquid == Terrain.VENOM) {
				return 3;
			} else if (Liquid == Terrain.LAVA) {
				return 4;
			} else if (Liquid == Terrain.OBSIDIAN) {
				return 5;
			} 

			return 0;
		}

		private int FromOverlay(int Ov) {
			if (Ov == 1) {
				return Terrain.ICE;
			} else if (Ov == 2) {
				return Terrain.WATER;
			} else if (Ov == 3) {
				return Terrain.VENOM;
			} else if (Ov == 4) {
				return Terrain.LAVA;
			} else if (Ov == 5) {
				return Terrain.OBSIDIAN;
			} 

			return Terrain.WATER;
		}

		public static bool IsValid(int I) {
			return I >= 0 && I < Size;
		}

		public Void SetOnFire(int I, bool Fire) {
			SetOnFire(I, Fire, true);
		}

		public Void SetOnFire(int I, bool Fire, bool GetWaterOut) {
			if (!IsValid(I)) {
				return;
			} 

			if (Fire && this is IceLevel) {
				return;
			} 

			byte T = this.Get(I);
			byte L = this.LiquidData[I];

			if (Fire && L == Terrain.ICE) {
				this.LiquidData[I] = Terrain.WATER;
				this.Info[I] = 0;
				int X = ToX(I) * 16;
				int Y = ToY(I) * 16 - 8;

				for (I = 0;
; I < 20; I++) {
					SteamFx Fx = new SteamFx();
					Fx.X = X + Random.NewFloat(16);
					Fx.Y = Y + Random.NewFloat(16);
					Dungeon.Area.Add(Fx);
				}

				return;
			} else if (Fire && L == Terrain.WATER && GetWaterOut) {
				this.LiquidData[I] = 0;
				this.Info[I] = 0;
				int X = ToX(I) * 16;
				int Y = ToY(I) * 16 - 8;

				for (int J = 0; J < 20; J++) {
					SteamFx Fx = new SteamFx();
					Fx.X = X + Random.NewFloat(16);
					Fx.Y = Y + Random.NewFloat(16);
					Dungeon.Area.Add(Fx);
				}

				X = ToX(I);
				Y = ToY(I);

				for (int Yy = Y - 1; Yy < Y + 2; Yy++) {
					for (int Xx = X - 1; Xx < X + 2; Xx++) {
						this.UpdateNeighbourMask(Xx, Yy);
						this.Tile(Xx, Yy);
					}
				}

				return;
			} 

			bool Ab = MatchesFlag(T, Terrain.BURNS);
			bool Bb = MatchesFlag(L, Terrain.BURNS);

			if ((((Ab && Bb) || Bb || (Ab && L == 0)) && L != Terrain.WATER && L != Terrain.EXIT)) {
				this.Info[I] = BitHelper.SetBit(this.Info[I], 0, Fire);
				this.Passable[I] = Fire ? false : CheckFor(I, Terrain.PASSABLE);
			} 
		}

		public Void Venom(int X, int Y) {
			int I = ToIndex(X, Y);

			if (this.LiquidData[I] == Terrain.WATER) {
				int Info = this.Info[I];

				if (BitHelper.GetNumber(Info, 10, 3) > 0) {
					return;
				} 

				Info = BitHelper.PutNumber(Info, 6, 4, 0);
				Info = BitHelper.SetBit(Info, 17, false);
				this.Info[I] = BitHelper.PutNumber(Info, 10, 3, 3);
				this.Passable[I] = false;

				for (int Yy = Y - 1; Yy < Y + 2; Yy++) {
					for (int Xx = X - 1; Xx < X + 2; Xx++) {
						this.UpdateNeighbourMask(Xx, Yy);
					}
				}
			} 
		}

		public Void ToObsidian(int I) {
			int Info = this.Info[I];

			if (BitHelper.GetNumber(Info, 10, 3) > 0) {
				return;
			} 

			Info = BitHelper.PutNumber(Info, 6, 4, 0);
			Info = BitHelper.SetBit(Info, 17, false);
			this.Info[I] = BitHelper.PutNumber(Info, 10, 3, 5);
			this.Passable[I] = true;
			int X = ToX(I);
			int Y = ToY(I);

			for (int Yy = Y - 1; Yy < Y + 2; Yy++) {
				for (int Xx = X - 1; Xx < X + 2; Xx++) {
					this.UpdateNeighbourMask(Xx, Yy);
				}
			}
		}

		public Void Freeze(int I) {
			byte L = this.LiquidData[I];
			int Info = this.Info[I];

			if (L == Terrain.WATER || L == Terrain.VENOM) {
				if (!BitHelper.IsBitSet(Info, 17) && BitHelper.GetNumber(Info, 10, 3) > 0) {
					return;
				} 

				Info = BitHelper.PutNumber(Info, 6, 4, 0);
				Info = BitHelper.SetBit(Info, 17, false);
				this.Info[I] = BitHelper.PutNumber(Info, 10, 3, 1);
				int X = ToX(I);
				int Y = ToY(I);

				for (int Yy = Y - 1; Yy < Y + 2; Yy++) {
					for (int Xx = X - 1; Xx < X + 2; Xx++) {
						this.UpdateNeighbourMask(Xx, Yy);
					}
				}
			} 
		}

		private Void UpdateNeighbourMask(int X, int Y) {
			int I = ToIndex(X, Y);
			int Info = this.Info[I];
			int Type;

			if (BitHelper.IsBitSet(Info, 17)) {
				Type = this.GetOverlayType(this.LiquidData[I]);
			} else {
				Type = BitHelper.GetNumber(Info, 10, 3);
			}


			int Count = 0;

			if (this.CheckForNeighbour(X, Y + 1, Type)) {
				Count += 1;
			} 

			if (this.CheckForNeighbour(X + 1, Y, Type)) {
				Count += 2;
			} 

			if (this.CheckForNeighbour(X, Y - 1, Type)) {
				Count += 4;
			} 

			if (this.CheckForNeighbour(X - 1, Y, Type)) {
				Count += 8;
			} 

			this.Info[I] = BitHelper.PutNumber(Info, 13, 4, Count);
		}

		private bool CheckForNeighbour(int X, int Y, int Type) {
			int I = ToIndex(X, Y);
			byte T = this.LiquidData[I];

			switch (Type) {
				case 1: {
					if (T == Terrain.ICE) {
						return true;
					} 

					break;
				}

				case 2: {
					if (T == Terrain.WATER) {
						return true;
					} 

					break;
				}

				case 3: {
					if (T == Terrain.VENOM) {
						return true;
					} 

					break;
				}

				case 4: {
					if (T == Terrain.LAVA) {
						return true;
					} 

					break;
				}

				case 5: {
					if (T == Terrain.OBSIDIAN) {
						return true;
					} 

					break;
				}
			}

			int Info = this.Info[I];

			return BitHelper.GetNumber(Info, 10, 3) == Type;
		}

		protected Void DoEffects() {
			if (this.LastFlame == 0) {
				OrthographicCamera Camera = Camera.Game;
				float Zoom = Camera.Zoom;
				float Cx = Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom;
				float Cy = Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom;
				int Sx = (int) (Math.Floor(Cx / 16) - 1);
				int Sy = (int) (Math.Floor(Cy / 16) - 1);
				int Fxx = (int) (Math.Ceil((Cx + Display.GAME_WIDTH * Zoom) / 16) + 1);
				int Fy = (int) (Math.Ceil((Cy + Display.GAME_HEIGHT * Zoom) / 16) + 1);

				for (int Y = Math.Min(Fy, GetHeight()) - 1; Y >= Math.Max(0, Sy); Y--) {
					for (int X = Math.Max(0, Sx); X < Math.Min(Fxx, GetWidth()); X++) {
						int I = X + Y * GetWidth();
						int Info = this.Info[I];

						if (BitHelper.IsBitSet(Info, 0)) {
							InGameState.Burning = true;
							TerrainFlameFx Fx = new TerrainFlameFx();
							Fx.X = X * 16 + Random.NewFloat(16);
							Fx.Y = Y * 16 - 8 + Random.NewFloat(16);
							Dungeon.Area.Add(Fx);

							if (Random.Chance(20)) {
								SteamFx S = new SteamFx();
								S.X = X * 16 + Random.NewFloat(16);
								S.Y = Y * 16 - 8 + Random.NewFloat(16);
								Dungeon.Area.Add(S);
								S.Val = Random.NewFloat(0, 0.5f);
							} 
						} else {
							byte Tile = this.Get(I);

							if (Tile == Terrain.CHASM) {
								if (Random.Chance(2f)) {
									Dungeon.Area.Add(new ChasmFx(Random.NewFloat(1f) * 16 + X * 16, Random.NewFloat(1f) * 16 + Y * 16 - 8));
								} 
							} else if (I > Width && this.Data[I - Width] == Terrain.CHASM) {
								byte T = this.LiquidData[I];

								if (T == Terrain.WATER || T == Terrain.LAVA) {
									WaterfallFx Fx = new WaterfallFx();
									Fx.X = X * 16 + Random.NewFloat(16);
									Fx.Y = Y * 16 - 6;
									Fx.Lava = T == Terrain.LAVA;
									Dungeon.Area.Add(Fx);
								} 
							} 

							if (this.LiquidData[I] == Terrain.LAVA && Random.Chance(2f)) {
								Dungeon.Area.Add(new LavaFx(Random.NewFloat(1f) * 16 + X * 16, Random.NewFloat(1f) * 16 + Y * 16 - 8));
							} 
						}

					}
				}
			} 
		}

		public int GetInfo(int I) {
			return Info[I];
		}

		private Void DoLogic() {
			foreach (Room Room in this.Rooms) {
				for (int Y = Room.Top; Y < Room.Bottom; Y++) {
					for (int X = Room.Left; X < Room.Right; X++) {
						int I = ToIndex(X, Y);
						int Info = this.Info[I];
						byte T = this.LiquidData[I];
						bool Burning = BitHelper.IsBitSet(Info, 0);

						if (Burning) {
							int Damage = BitHelper.GetNumber(Info, 1, 4) + 1;

							if (Damage > 1) {
								foreach (int J in PathFinder.NEIGHBOURS4) {
									this.SetOnFire(J + I, true);
								}
							} 

							if (Damage == 3) {
								this.Info[I] = 0;

								if (MatchesFlag(this.Get(X, Y), Terrain.BURNS)) {
									this.Data[I] = Terrain.FLOOR_A;
								} 

								this.LiquidData[I] = Terrain.EMBER;
								this.UpdateTile(X, Y);
								this.Passable[I] = true;
							} else {
								this.Info[I] = (byte) BitHelper.PutNumber(Info, 1, 4, Damage);
							}

						} 

						if (T == Terrain.ICE) {
							foreach (int J in PathFinder.NEIGHBOURS4) {
								this.Freeze(I + J);
							}
						} else if (T == Terrain.GRASS || T == Terrain.HIGH_GRASS) {
							if (Dungeon.Depth != -3 && (UpdateId + X + Y) % 20 == 0) {
								if (T == Terrain.GRASS && Random.Chance(1)) {
									this.Set(I, Terrain.HIGH_GRASS);
								} 

								I += PathFinder.NEIGHBOURS8[Random.NewInt(8)];

								if (this.LiquidData[I] == Terrain.DIRT) {
									this.Set(I, Terrain.GRASS);
									this.UpdateTile(ToX(I), ToY(I));
								} 
							} 
						} else if (T == Terrain.LAVA) {
							foreach (int J in PathFinder.NEIGHBOURS4) {
								int K = J + I;
								byte L = this.LiquidData[K];

								if (L == Terrain.WATER || L == Terrain.VENOM) {
									this.ToObsidian(I);
								} else if (L == Terrain.ICE) {
									this.SetOnFire(K, true);
								} 
							}
						} else if (T == Terrain.VENOM) {
							foreach (int J in PathFinder.NEIGHBOURS4) {
								this.Venom(ToX(I + J), ToY(I + J));
							}
						} 
					}
				}
			}
		}

		public Void RenderSolid() {
			if (Dungeon.Level != this) {
				return;
			} 

			foreach (Room Room in this.Rooms) {
				Room.LastNumEnemies = Room.NumEnemies;
				Room.NumEnemies = 0;
			}

			OrthographicCamera Camera = Camera.Game;
			float Zoom = Camera.Zoom;
			float Cx = Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom;
			float Cy = Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom;
			int Sx = (int) (Math.Floor(Cx / 16) - 1);
			int Sy = (int) (Math.Floor(Cy / 16) - 1);
			int Fx = (int) (Math.Ceil((Cx + Display.GAME_WIDTH * Zoom) / 16) + 1);
			int Fy = (int) (Math.Ceil((Cy + Display.GAME_HEIGHT * Zoom) / 16) + 1);

			for (int Y = Math.Min(Fy, GetHeight()) - 1; Y >= Math.Max(0, Sy); Y--) {
				for (int X = Math.Max(0, Sx); X < Math.Min(Fx, GetWidth()); X++) {
					int I = X + Y * GetWidth();

					if (!this.Low[I] && (this.Light[I] > 0 || this.Light[I + GetWidth()] > 0)) {
						byte Tile = this.Get(I);

						if (Terrain.Patterns[Tile] != null) {
							TextureRegion Region = new TextureRegion(Terrain.Patterns[Tile]);
							int N = Region.GetRegionHeight() / 16;
							Region.SetRegionX(Region.GetRegionX() + X % (Region.GetRegionWidth() / 16) * 16);
							Region.SetRegionY(Region.GetRegionY() + (N - 1 - (Y % N)) * 16);
							Region.SetRegionWidth(16);
							Region.SetRegionHeight(16);
							Graphics.Render(Region, X * 16, Y * 16);
						} 

						if (Tile != Terrain.WALL && Tile != Terrain.CRACK && Terrain.Variants[Tile] != null) {
							byte Variant = this.Variants[I];

							if (Variant != Terrain.Variants[Tile].Length && Terrain.Variants[Tile][Variant] != null) {
								Graphics.Render(Terrain.Variants[Tile][Variant], X * 16, Y * 16 - 8);
							} 
						} 

						if (Tile == Terrain.WALL || Tile == Terrain.CRACK) {
							short V = this.Walls[I];

							for (int Yy = 0; Yy < 2; Yy++) {
								for (int Xx = 0; Xx < 2; Xx++) {
									int Lv = 0;

									if (Yy == 0 || !IsBitSet(V, 0)) {
										Lv += 1;
									} 

									if (Xx == 0 || !IsBitSet(V, 1)) {
										Lv += 2;
									} 

									if (Yy > 0 || !IsBitSet(V, 2)) {
										Lv += 4;
									} 

									if (Xx > 0 || !IsBitSet(V, 3)) {
										Lv += 8;
									} 

									if (Lv == 15) {
										Lv = 0;

										if (Xx == 1 && Yy == 1 && IsBitSet(V, 4)) {
											Lv += 1;
										} 

										if (Xx == 1 && Yy == 0 && IsBitSet(V, 5)) {
											Lv += 2;
										} 

										if (Xx == 0 && Yy == 0 && IsBitSet(V, 6)) {
											Lv += 4;
										} 

										if (Xx == 0 && Yy == 1 && IsBitSet(V, 7)) {
											Lv += 8;
										} 

										int Vl = Terrain.WallMapExtra[Lv];

										if (Vl != -1) {
											float A = GetLight(X + (Xx == 0 ? -1 : 1), Y + Yy);

											if (A > 0.05f) {
												Graphics.Batch.SetColor(1, 1, 1, A);
												Graphics.Render(Terrain.WallTop[this.WallDecor[I]][Vl], X * 16 + Xx * 8, Y * 16 + Yy * 8);
											} 
										} 
									} else {
										int Vl = Terrain.WallMap[Lv];

										if (Vl != -1) {
											float A = GetLight(X + (Xx == 0 ? -1 : 1), Y + Yy);

											if (A > 0.05f) {
												Graphics.Batch.SetColor(1, 1, 1, A);
												Graphics.Render(Terrain.WallTop[this.WallDecor[I]][Vl], X * 16 + Xx * 8, Y * 16 + Yy * 8);
											} 
										} 
									}

								}
							}
						} 
					} 

					Graphics.Batch.SetColor(1, 1, 1, 1);

					if (RENDER_PASSABLE) {
						if (this.Passable[I]) {
							Graphics.Batch.End();
							Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
							Graphics.Shape.SetColor(0.5f, 0.5f, 0.5f, 1);
							Graphics.Shape.Begin(ShapeRenderer.ShapeType.Line);
							Graphics.Shape.Rect(X * 16 + 1, Y * 16 + 1 - 8, 16 - 2, 16 - 2);
							Graphics.Shape.End();
							Graphics.Batch.Begin();
						} 
					} 
				}
			}

			if (RENDER_ROOM_DEBUG) {
				Graphics.Batch.End();
				Gdx.Gl.GlEnable(GL20.GL_BLEND);
				Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
				Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
				Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);

				foreach (Room Room in this.Rooms) {
					Graphics.Shape.SetColor(Room.Hidden ? 0 : 1, Room == Player.Instance.Room ? 0 : 1, Room == Player.Instance.Room ? 0 : 1, 0.1f);
					Graphics.Shape.Rect(Room.Left * 16 + 8, Room.Top * 16 + 8, Room.GetWidth() * 16 - 16, Room.GetHeight() * 16 - 16);
				}

				Graphics.Shape.End();
				Gdx.Gl.GlDisable(GL20.GL_BLEND);
				Graphics.Batch.Begin();

				foreach (Room Room in this.Rooms) {
					Graphics.Print(Room.GetClass().GetSimpleName(), Graphics.Small, Room.Left * 16 + 16, Room.Top * 16 + 16);
				}
			} 
		}

		private Void RenderFloor(int Sx, int Sy, int Fx, int Fy) {
			if (this.Low == null || this.Light == null) {
				this.LoadPassable();
			} 

			for (int Y = Math.Max(0, Sy); Y < Math.Min(Fy, GetHeight()); Y++) {
				for (int X = Math.Max(0, Sx); X < Math.Min(Fx, GetWidth()); X++) {
					int I = X + Y * GetWidth();

					if (this.Low[I] && this.Light[I] > 0) {
						byte Tile = this.Get(I);

						if (Tile == Terrain.DISCO) {
							TextureRegion Region = new TextureRegion(Terrain.DiscoPattern);
							Region.SetRegionX(Region.GetRegionX() + (X + Y + (this.T % 2f >= 1f ? 1 : 0)) % 2 * 16);
							Region.SetRegionWidth(16);
							Graphics.Render(Region, X * 16, Y * 16 - 8);
						} else if (Terrain.Patterns[Tile] != null) {
							TextureRegion Region = new TextureRegion(Terrain.Patterns[Tile]);
							int W = Region.GetRegionWidth() / 16;
							int H = Region.GetRegionHeight() / 16;
							Region.SetRegionX(Region.GetRegionX() + X % W * 16);
							Region.SetRegionY(Region.GetRegionY() + (H - 1 - Y % H) * 16);
							Region.SetRegionWidth(16);
							Region.SetRegionHeight(16);
							Graphics.Render(Region, X * 16, Y * 16 - 8);
						} 

						if (Terrain.Variants[Tile] != null) {
							byte Variant = this.Variants[I];

							if (Variant != Terrain.Variants[Tile].Length && Terrain.Variants[Tile][Variant] != null) {
								Graphics.Render(Terrain.Variants[Tile][Variant], X * 16, Y * 16 - 8);
							} 
						} 
					} 
				}
			}
		}

		public Void RenderLiquids() {
			OrthographicCamera Camera = Camera.Game;
			float Zoom = Camera.Zoom;
			float Cx = Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom;
			float Cy = Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom;
			int Sx = (int) (Math.Floor(Cx / 16) - 1);
			int Sy = (int) (Math.Floor(Cy / 16) - 1);
			int Fx = (int) (Math.Ceil((Cx + Display.GAME_WIDTH * Zoom) / 16) + 1);
			int Fy = (int) (Math.Ceil((Cy + Display.GAME_HEIGHT * Zoom) / 16) + 1);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(MaskShader);
			Graphics.Batch.Begin();

			for (int Y = Math.Min(Fy, GetHeight()) - 1; Y >= Math.Max(0, Sy); Y--) {
				for (int X = Math.Max(0, Sx); X < Math.Min(Fx, GetWidth()); X++) {
					int I = X + Y * GetWidth();

					if (this.Light[I] == 0) {
						continue;
					} 

					int Info = this.Info[I];
					byte Tile = this.LiquidData[I];

					if (BitHelper.IsBitSet(Info, 17)) {
						this.DoDraw((byte) this.FromOverlay(BitHelper.GetNumber(Info, 10, 3)), I, X, Y);

						if (Tile == Terrain.ICE) {
							DrawOver(Terrain.IcePattern, I, X, Y, false, Info);
						} else if (Tile == Terrain.VENOM) {
							DrawOver(Terrain.VenomPattern, I, X, Y, true, Info);
						} else if (Tile == Terrain.OBSIDIAN) {
							DrawOver(Terrain.ObsidianPattern, I, X, Y, false, Info);
						} 
					} else {
						this.DoDraw(Tile, I, X, Y);
						int Spread = BitHelper.GetNumber(Info, 10, 3);

						if (Spread > 0) {
							int Step = BitHelper.GetNumber(Info, 6, 4);
							TextureRegion R;

							if (Spread == 1) {
								R = new TextureRegion(Terrain.IcePattern);
							} else if (Spread == 3) {
								R = new TextureRegion(Terrain.VenomPattern);
							} else if (Spread == 5) {
								R = new TextureRegion(Terrain.ObsidianPattern);
							} else {
								continue;
							}


							R.SetRegionX(R.GetRegionX() + X % 4 * 16);
							R.SetRegionY(R.GetRegionY() + (3 - Y % 4) * 16);
							int Rx = R.GetRegionX();
							int Ry = R.GetRegionY();
							R.SetRegionHeight(16);
							R.SetRegionWidth(16);
							Texture Texture = R.GetTexture();
							int Rw = Texture.GetWidth();
							int Rh = Texture.GetHeight();
							TextureRegion Rr = Terrain.Spread[BitHelper.GetNumber(Info, 13, 4)];
							Texture T = Rr.GetTexture();
							Graphics.Batch.End();
							MaskShader.Begin();
							MaskShader.SetUniformf("spreadStep", ((float) Step) / 256f);
							T.Bind(1);
							MaskShader.SetUniformi("u_texture2", 1);
							MaskShader.SetUniformf("activated", 1);
							MaskShader.SetUniformf("spread", 1);
							MaskShader.SetUniformf("water", Spread == 3 ? 1 : 0);
							MaskShader.SetUniformf("tpos", new Vector2(((float) Rr.GetRegionX()) / Rw, ((float) Rr.GetRegionY()) / Rh));
							MaskShader.SetUniformf("time", this.T);
							MaskShader.SetUniformf("pos", new Vector2(((float) Rx) / Rw, ((float) Ry) / Rh));
							MaskShader.SetUniformf("size", new Vector2(16f / Rw, 16f / Rh));
							Texture.Bind(0);
							MaskShader.SetUniformi("u_texture", 1);
							Rr = Terrain.Edges[Tile][this.LiquidVariants[I]];
							MaskShader.SetUniformf("epos", new Vector2(((float) Rr.GetRegionX()) / Rw, ((float) Rr.GetRegionY()) / Rh));
							MaskShader.End();
							Graphics.Batch.Begin();
							Graphics.Render(R, X * 16, Y * 16 - 8);
							Graphics.Batch.End();
							MaskShader.Begin();
							MaskShader.SetUniformf("spread", 0);
							MaskShader.SetUniformf("activated", 0);
							MaskShader.End();
							Graphics.Batch.Begin();
						} 
					}

				}
			}

			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();

			for (int Y = Math.Min(Fy, GetHeight()) - 1; Y >= Math.Max(0, Sy); Y--) {
				for (int X = Math.Max(0, Sx); X < Math.Min(Fx, GetWidth()); X++) {
					int I = X + Y * GetWidth();
					byte Tile = this.Get(I);

					if (this.Light[I] > 0 && I >= GetWidth() && (Tile == Terrain.WALL || Tile == Terrain.CRACK)) {
						byte T = this.Get(I - GetWidth());

						if (T != Terrain.CRACK && T != Terrain.WALL) {
							Graphics.StartShadows();
							Graphics.Render(Terrain.TopVariants[0], X * 16, Y * 16 - 1, 0, 0, 0, false, false, 1f, -1f);
							Graphics.EndShadows();
						} 
					} 
				}
			}

			RenderShadows();
		}

		private Void DoDraw(byte Tile, int I, int X, int Y) {
			if (Tile == Terrain.EXIT) {
				float Dt = Gdx.Graphics.GetDeltaTime();
				Exit.Al = MathUtils.Clamp(0, 1, Exit.Al + ((Exit.ExitFx != null ? 1 : 0) - Exit.Al) * Dt * 10);

				if (Exit.Al > 0) {
					Graphics.Batch.End();
					Mob.Shader.Begin();
					Mob.Shader.SetUniformf("u_color", ColorUtils.WHITE);
					Mob.Shader.SetUniformf("u_a", Exit.Al);
					Mob.Shader.End();
					Graphics.Batch.SetShader(Mob.Shader);
					Graphics.Batch.Begin();

					for (int Yy = -1; Yy < 2; Yy++) {
						for (int Xx = -1; Xx < 2; Xx++) {
							if (Math.Abs(Xx) + Math.Abs(Yy) == 1) {
								Graphics.Render(Terrain.Exit, X * 16 + Xx, Y * 16 - 7 + Yy);
							} 
						}
					}

					Graphics.Batch.End();
					Graphics.Batch.SetShader(MaskShader);
					Graphics.Batch.Begin();
				} 

				Graphics.Render(Terrain.Exit, X * 16, Y * 16 - 7);
			} else if (Tile == Terrain.WATER) {
				InGameState.Flow = true;
				DrawWith(Terrain.WaterPattern, Terrain.Pooledge, I, X, Y, true);
			} else if (Tile == Terrain.LAVA) {
				DrawWith(Terrain.LavaPattern, Terrain.Lavaedge, I, X, Y, true);
			} else if (Tile == Terrain.VENOM) {
				DrawWith(Terrain.VenomPattern, Terrain.Pooledge, I, X, Y, true);
			} else if (Tile == Terrain.HIGH_GRASS) {
				float A = (float) (Math.Cos(this.T + (X + Y) / 2f + Y / 4f) * 20f * Math.Sin(this.T * 0.75f + X / 3f - Y / 6f));
				Graphics.Render(Terrain.GrassHigh, X * 16 + 8, Y * 16 - 8, A, 8, 0, false, false);
			} else if (Tile == Terrain.HIGH_DRY_GRASS) {
				Graphics.Render(Terrain.DryGrassHigh, X * 16, Y * 16 - 8);
			} else if (Tile == Terrain.OBSIDIAN) {
				DrawWith(Terrain.ObsidianPattern, Terrain.Dirtedge, I, X, Y, false);
			} 
		}

		private Void DrawOver(TextureRegion Pattern, int I, int X, int Y, bool Water, int Info) {
			TextureRegion R = new TextureRegion(Pattern);
			R.SetRegionX(R.GetRegionX() + X % 4 * 16);
			R.SetRegionY(R.GetRegionY() + (3 - Y % 4) * 16);
			int Rx = R.GetRegionX();
			int Ry = R.GetRegionY();
			R.SetRegionHeight(16);
			R.SetRegionWidth(16);
			Texture Texture = R.GetTexture();
			int Rw = Texture.GetWidth();
			int Rh = Texture.GetHeight();
			TextureRegion Rr = Terrain.Spread[BitHelper.GetNumber(Info, 13, 4)];
			Texture T = Rr.GetTexture();
			Graphics.Batch.End();
			MaskShader.Begin();
			MaskShader.SetUniformf("spreadStep", 1f);
			T.Bind(1);
			MaskShader.SetUniformi("u_texture2", 1);
			MaskShader.SetUniformf("activated", 1);
			MaskShader.SetUniformf("spread", 1);
			MaskShader.SetUniformf("water", Water ? 1 : 0);

			if (Water) {
				MaskShader.SetUniformf("a", Pattern == Terrain.WaterPattern ? 0.5f : 1);
			} 

			MaskShader.SetUniformf("tpos", new Vector2(((float) Rr.GetRegionX()) / Rw, ((float) Rr.GetRegionY()) / Rh));
			MaskShader.SetUniformf("time", this.T);
			MaskShader.SetUniformf("pos", new Vector2(((float) Rx) / Rw, ((float) Ry) / Rh));
			MaskShader.SetUniformf("size", new Vector2(16f / Rw, 16f / Rh));
			Texture.Bind(0);
			MaskShader.SetUniformi("u_texture", 1);
			Rr = Terrain.Edges[FromOverlay(BitHelper.GetNumber(Info, 10, 3))][this.LiquidVariants[I]];
			MaskShader.SetUniformf("epos", new Vector2(((float) Rr.GetRegionX()) / Rw, ((float) Rr.GetRegionY()) / Rh));
			MaskShader.End();
			Graphics.Batch.Begin();
			Graphics.Render(R, X * 16, Y * 16 - 8);
			Graphics.Batch.End();
			MaskShader.Begin();
			MaskShader.SetUniformf("spread", 0);
			MaskShader.SetUniformf("activated", 0);
			MaskShader.End();
			Graphics.Batch.Begin();
		}

		private Void DrawWith(TextureRegion Pattern, TextureRegion Edge, int I, int X, int Y, bool Water) {
			byte Variant = this.LiquidVariants[I];
			TextureRegion R = new TextureRegion(Pattern);
			R.SetRegionX(R.GetRegionX() + X % 4 * 16);
			R.SetRegionY(R.GetRegionY() + (3 - Y % 4) * 16);
			int Rx = R.GetRegionX();
			int Ry = R.GetRegionY();
			R.SetRegionHeight(16);
			R.SetRegionWidth(16);
			Texture Texture = R.GetTexture();
			int Rw = Texture.GetWidth();
			int Rh = Texture.GetHeight();
			TextureRegion Rr = Edge[Variant];
			Texture T = Rr.GetTexture();
			Graphics.Batch.End();
			MaskShader.Begin();
			T.Bind(1);
			MaskShader.SetUniformf("activated", 1);
			MaskShader.SetUniformf("water", Water ? 1 : 0);

			if (Water) {
				MaskShader.SetUniformf("a", Pattern == Terrain.WaterPattern ? 0.5f : 1);
			} 

			MaskShader.SetUniformf("speed", Pattern == Terrain.LavaPattern ? 0.4f : 1);
			MaskShader.SetUniformi("u_texture2", 1);
			MaskShader.SetUniformf("tpos", new Vector2(((float) Rr.GetRegionX()) / Rw, ((float) Rr.GetRegionY()) / Rh));
			Texture.Bind(0);
			MaskShader.SetUniformi("u_texture", 1);
			MaskShader.SetUniformf("time", this.T);
			MaskShader.SetUniformf("pos", new Vector2(((float) Rx) / Rw, ((float) Ry) / Rh));
			MaskShader.SetUniformf("size", new Vector2(16f / Rw, 16f / Rh));
			MaskShader.End();
			Graphics.Batch.Begin();

			if (Water) {
				Graphics.Render(R, X * 16, Y * 16 - 8);
			} else {
				Graphics.Render(R, X * 16, Y * 16 - 8);
			}


			Graphics.Batch.End();
			MaskShader.Begin();
			MaskShader.SetUniformf("activated", 0);
			MaskShader.End();
			Graphics.Batch.Begin();
		}

		private Void RenderShadows() {
			if (SHADOWS) {
				float Zoom = Camera.Game.Zoom;
				Graphics.Batch.SetProjectionMatrix(Camera.Game.Combined);
				Graphics.Batch.SetColor(0, 0, 0, 0.5f);
				Texture Texture = Graphics.Shadows.GetColorBufferTexture();
				Texture.SetFilter(Texture.TextureFilter.Linear, Texture.TextureFilter.Linear);
				Graphics.Batch.Draw(Texture, Camera.Game.Position.X - Display.GAME_WIDTH / 2 * Zoom, Camera.Game.Position.Y - Display.GAME_HEIGHT / 2 * Zoom, Display.GAME_WIDTH * Zoom, Display.GAME_HEIGHT * Zoom, 0, 0, Texture.GetWidth(), Texture.GetHeight(), false, true);
				Graphics.Batch.SetColor(1, 1, 1, 1f);
			} 
		}

		public Void RenderSides() {
			OrthographicCamera Camera = Camera.Game;
			float Zoom = Camera.Zoom;
			float Cx = Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom;
			float Cy = Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom;
			int Sx = (int) (Math.Floor(Cx / 16) - 1);
			int Sy = (int) (Math.Floor(Cy / 16) - 1);
			int Fx = (int) (Math.Ceil((Cx + Display.GAME_WIDTH * Zoom) / 16) + 1);
			int Fy = (int) (Math.Ceil((Cy + Display.GAME_HEIGHT * Zoom) / 16) + 1);

			for (int Y = Math.Min(Fy, GetHeight()) - 1; Y >= Math.Max(0, Sy); Y--) {
				for (int X = Math.Max(0, Sx); X < Math.Min(Fx, GetWidth()); X++) {
					int I = X + Y * GetWidth();

					if (I >= Data.Length) {
						continue;
					} 

					byte Tile = this.Get(I);

					if (this.Light[I] > 0 && I >= GetWidth()) {
						if ((Tile == Terrain.WALL || Tile == Terrain.CRACK)) {
							byte Left = this.Get(I - 1);
							byte Right = this.Get(I + 1);
							bool Lg = (Left == Terrain.WALL || Left == Terrain.CRACK);
							bool Rg = (Right == Terrain.WALL || Right == Terrain.CRACK);
							byte T = this.Get(I - GetWidth());

							if (T != Terrain.CRACK && T != Terrain.WALL) {
								Graphics.Render(Terrain.TopVariants[(X * 3 + Y / 2 + (X + Y) / 2) % 12], X * 16, Y * 16 - 16);

								if (!Lg || !Rg) {
									T = 1;

									if (Lg) {
										T = 0;
									} else if (Rg) {
										T = 2;
									} 

									Graphics.Render(Terrain.Sides[T], X * 16, Y * 16 - 16);
								} 
							} 

							int M = I - GetWidth();

							if (M > -1) {
								T = this.Get(M);
							} 
						} else if (Tile == Terrain.CHASM) {
							Graphics.Render(Terrain.ChasmPattern, X * 16, Y * 16 - 8);

							if (Data[I - 1] != Terrain.CHASM && Data[I - 1] != Terrain.WALL) {
								Graphics.Render(Terrain.ChasmSides[3][(Y + X * 2) % 3], X * 16 - 16, Y * 16 - 8);
							} 

							if (Data[I + 1] != Terrain.CHASM && Data[I + 1] != Terrain.WALL) {
								Graphics.Render(Terrain.ChasmSides[1][(Y + X * 2) % 3], X * 16 + 16, Y * 16 - 8);
							} 

							if (Data[I - Width] != Terrain.CHASM && Data[I - Width] != Terrain.WALL) {
								Graphics.Render(Terrain.ChasmSides[2][(Y + X * 2 - 1) % 3], X * 16, Y * 16 - 24);
							} 
						} 
					} 
				}
			}

			Graphics.Batch.End();
			Graphics.Batch.SetShader(Shader);
			Graphics.Batch.Begin();

			for (int Y = Math.Min(Fy, GetHeight()) - 1; Y >= Math.Max(0, Sy); Y--) {
				for (int X = Math.Max(0, Sx); X < Math.Min(Fx, GetWidth()); X++) {
					int I = X + Y * GetWidth();

					if (I >= Data.Length) {
						continue;
					} 

					byte Tile = this.Get(I);

					if (this.Light[I] > 0 && I >= GetWidth()) {
						if (Tile == Terrain.CHASM && this.Get(I + GetWidth()) != Terrain.CHASM) {
							Graphics.Batch.End();
							Shader.Begin();
							TextureRegion Reg = Terrain.TopVariants[(X * 3 + Y / 2 + (X + Y) / 2) % 12];
							Texture Texture = Reg.GetTexture();
							Shader.SetUniformf("pos", new Vector2(((float) Reg.GetRegionX()) / Texture.GetWidth(), ((float) Reg.GetRegionY()) / Texture.GetHeight()));
							Shader.SetUniformf("size", new Vector2(((float) Reg.GetRegionWidth()) / Texture.GetWidth(), ((float) Reg.GetRegionHeight()) / Texture.GetHeight()));
							Shader.End();
							Graphics.Batch.Begin();
							Graphics.Render(Reg, X * 16, Y * 16 - 8);
						} 
					} 
				}
			}

			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public override Void Render() {
			if (Dungeon.Level != this) {
				SetDone(true);

				return;
			} 

			OrthographicCamera Camera = Camera.Game;
			float Zoom = Camera.Zoom;
			float Cx = Camera.Position.X - Display.GAME_WIDTH / 2 * Zoom;
			float Cy = Camera.Position.Y - Display.GAME_HEIGHT / 2 * Zoom;
			int Sx = (int) (Math.Floor(Cx / 16) - 1);
			int Sy = (int) (Math.Floor(Cy / 16) - 1);
			int Fx = (int) (Math.Ceil((Cx + Display.GAME_WIDTH * Zoom) / 16) + 1);
			int Fy = (int) (Math.Ceil((Cy + Display.GAME_HEIGHT * Zoom) / 16) + 1);
			RenderFloor(Sx, Sy, Fx, Fy);
			Graphics.Batch.End();
			Graphics.Batch.SetShader(MaskShader);
			Graphics.Batch.Begin();

			for (int Y = Math.Min(Fy, GetHeight()) - 1; Y >= Math.Max(0, Sy); Y--) {
				for (int X = Math.Max(0, Sx); X < Math.Min(Fx, GetWidth()); X++) {
					int I = X + Y * GetWidth();

					if (this.Light[I] == 0) {
						continue;
					} 

					byte Tile = this.LiquidData[I];

					if (Tile == Terrain.EXIT) {
						Graphics.Render(Terrain.Exit, X * 16, Y * 16 - 8);
					} else if (Tile == Terrain.DIRT) {
						DrawWith(Terrain.DirtPattern, Terrain.Dirtedge, I, X, Y, false);
					} else if (Tile == Terrain.GRASS || Tile == Terrain.DRY_GRASS || Tile == Terrain.HIGH_GRASS || Tile == Terrain.HIGH_DRY_GRASS) {
						bool Dry = (Tile == Terrain.DRY_GRASS || Tile == Terrain.HIGH_DRY_GRASS);
						DrawWith(Dry ? Terrain.DryGrassPattern : Terrain.GrassPattern, Dry ? Terrain.Drygrassedge : Terrain.Grassedge, I, X, Y, false);
					} else if (Tile == Terrain.COBWEB) {
						DrawWith(Terrain.CobwebPattern, Terrain.Webedge, I, X, Y, false);
					} else if (Tile == Terrain.EMBER) {
						Graphics.Batch.End();
						Graphics.Batch.SetShader(null);
						Graphics.Batch.Begin();
						TextureRegion R = new TextureRegion(Tile == Terrain.EMBER ? Terrain.EmberPattern : Terrain.CobwebPattern);
						R.SetRegionX(R.GetRegionX() + X % 4 * 16);
						R.SetRegionY(R.GetRegionY() + (3 - Y % 4) * 16);
						R.SetRegionHeight(16);
						R.SetRegionWidth(16);
						Graphics.Render(R, X * 16, Y * 16 - 8);
						Graphics.Batch.End();
						Graphics.Batch.SetShader(MaskShader);
						Graphics.Batch.Begin();
					} else if (Tile == Terrain.ICE) {
						DrawWith(Terrain.IcePattern, Terrain.Pooledge, I, X, Y, false);
					} 
				}
			}

			Graphics.Batch.End();
			Graphics.Batch.SetShader(null);
			Graphics.Batch.Begin();
		}

		public Void AddLight(float X, float Y, float A, float Max) {
			float Dt = Gdx.Graphics.GetDeltaTime();
			int I = (int) (Math.Floor(X / 16) + Math.Floor(Y / 16) * GetWidth());

			if (I < 0 || I >= GetSize()) {
				return;
			} 

			if (this.Light[I] < Max) {
				this.Light[I] = Math.Min(1, this.Light[I] + A * Dt);

				if (this.Light[I] >= 0.4f) {
					this.Explored[I] = true;
				} 
			} 
		}

		public float GetLight(int I) {
			if (I < 0 || I >= GetSize()) {
				return 0;
			} 

			return this.Light[I];
		}

		public float GetLight(int X, int Y) {
			int I = ToIndex(X, Y);

			if (I < 0 || I >= GetSize()) {
				return 0;
			} 

			return this.Light[I];
		}

		public Void AddLightInRadius(float X, float Y, float A, float Rd, bool Xray) {
			int Fx = (int) Math.Floor((X) / 16);
			int Fy = (int) Math.Floor((Y) / 16);

			if (Fx < 0 || Fy < 0) {
				return;
			} 

			for (int Yy = (int) -Rd; Yy <= Rd; Yy++) {
				for (int Xx = (int) -Rd; Xx <= Rd; Xx++) {
					if (Xx + Fx < 0 || Yy + Fy < 0 || Xx + Fx >= Level.GetWidth() || Yy + Fy >= Level.GetHeight()) {
						continue;
					} 

					float D = (float) Math.Sqrt(Xx * Xx + Yy * Yy);

					if (D < Rd) {
						bool See = Xray;
						float V = 1;

						if (!See) {
							if (this.IsValid(Fx + Xx, Fy + Yy) && this.IsValid(Fx, Fy)) {
								byte Vl = this.CanSee(Fx, Fy, Fx + Xx, Fy + Yy);

								if (Vl == 1 && Yy >= 0) {
									See = true;
								} else {
									See = Vl == 0;
								}

							} 
						} 

						if (See) {
							Dungeon.Level.AddLight(X + Xx * 16, Y + Yy * 16, A, (Rd - D) / Rd * V);
						} 
					} 
				}
			}
		}

		public byte GetData() {
			return this.Data;
		}

		public Void SetData(byte Data) {
			this.Data = Data;
		}

		public byte CanSee(int X, int Y, int Px, int Py) {
			return CanSee(X, Y, Px, Py, 0);
		}

		public byte CanSee(int X, int Y, int Px, int Py, int Extra) {
			Line Line = new Line(X, Y, Px, Py);
			bool First = false;

			foreach (Point Point in Line.GetPoints()) {
				if (First) {
					return 2;
				} 

				if (this.CheckFor((int) Point.X, (int) Point.Y, Terrain.BREAKS_LOS)) {
					First = true;
				} else if (Extra != 0 && this.CheckFor((int) Point.X, (int) Point.Y, Extra)) {
					First = true;
				} 
			}

			if (First) {
				return 1;
			} 

			return 0;
		}

		public bool IsValid(int X, int Y) {
			return !(X < 0 || Y < 0 || X >= GetWidth() || Y >= GetHeight());
		}

		public static bool MatchesFlag(byte B, int Flag) {
			if (B < 0) {
				return false;
			} 

			if (Flag == Terrain.BURNS) {
				if (Dungeon.Level is TechLevel && (B == Terrain.FLOOR_B)) {
					return false;
				} 

				if (Dungeon.Level is LibraryLevel && (B == Terrain.FLOOR_A || B == Terrain.FLOOR_B || B == Terrain.FLOOR_C)) {
					return true;
				} 

				if (Dungeon.Level is ForestLevel && (B == Terrain.FLOOR_A || B == Terrain.FLOOR_B)) {
					return true;
				} 
			} 

			return (Terrain.Flags[B] & Flag) == Flag;
		}

		public bool CheckFor(int I, int Flag) {
			if (I < 0 || I > GetSize()) {
				return false;
			} 

			if (Flag == Terrain.PASSABLE && this.LiquidData[I] == Terrain.LAVA) {
				return false;
			} else if (Flag == Terrain.BREAKS_LOS && (this.LiquidData[I] == Terrain.HIGH_DRY_GRASS || this.LiquidData[I] == Terrain.HIGH_GRASS)) {
				return true;
			} 

			return MatchesFlag(this.Get(I), Flag);
		}

		public bool CheckFor(int X, int Y, int Flag) {
			return CheckFor(ToIndex(X, Y), Flag);
		}

		public Void Set(int I, byte V) {
			if (MatchesFlag(V, Terrain.LIQUID_LAYER)) {
				this.LiquidData[I] = V;

				if (this.Get(I) == Terrain.CHASM) {
					this.Data[I] = Terrain.FLOOR_A;
				} 
			} else {
				if (this.LiquidData[I] != Terrain.PORTAL) {
					this.Data[I] = V;
				} 

				this.LiquidData[I] = 0;
			}

		}

		public Void Set(int X, int Y, byte V) {
			int I = ToIndex(X, Y);

			if (MatchesFlag(V, Terrain.LIQUID_LAYER)) {
				this.LiquidData[I] = V;

				if (this.Get(I) == Terrain.CHASM) {
					this.Data[I] = Terrain.FLOOR_A;
				} 
			} else {
				if (this.LiquidData[I] != Terrain.PORTAL) {
					this.Data[I] = V;
				} 
			}

		}

		public byte Get(int I) {
			byte T = this.Data[I];

			return T < 0 ? Terrain.WALL : T;
		}

		public Void Hide(int X, int Y) {
			this.Data[ToIndex(X, Y)] = (byte) -this.Data[ToIndex(X, Y)];
		}

		public byte Get(int X, int Y) {
			return Get(ToIndex(X, Y));
		}

		public abstract Void Generate(int Attempt);

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
			this.Chasms = World.RemoveBody(this.Chasms);
		}

		public bool Same(Level Level) {
			return this.GetClass().IsInstance(Level);
		}

		public Void AddPhysics() {
			Log.Physics("Creating level body");
			this.Body = World.RemoveBody(this.Body);
			this.Chasms = World.RemoveBody(this.Chasms);
			BodyDef Def = new BodyDef();
			Def.Type = BodyDef.BodyType.StaticBody;
			this.Body = World.World.CreateBody(Def);
			BodyDef Cdef = new BodyDef();
			Cdef.Type = BodyDef.BodyType.StaticBody;
			this.Chasms = World.World.CreateBody(Cdef);
			this.Chasms.SetUserData(this);

			for (int Y = 0; Y < GetHeight(); Y++) {
				for (int X = 0; X < GetWidth(); X++) {
					if (this.CheckFor(X, Y, Terrain.SOLID)) {
						int Total = 0;

						foreach (Vector2 Vec in NEIGHBOURS8V) {
							Vector2 V = new Vector2(X + Vec.X, Y + Vec.Y);

							if (this.IsValid((int) V.X, (int) V.Y) && (this.CheckFor((int) V.X, (int) V.Y, Terrain.SOLID))) {
								Total++;
							} 
						}

						if (Total < 8) {
							PolygonShape Poly = new PolygonShape();
							int Xx = X * 16;
							int Yy = Y * 16;

							if (this.CheckFor(X, Y + 1, Terrain.SOLID) || this.CheckFor(X, Y + 1, Terrain.HOLE)) {
								List<Vector2> Array = new List<>();
								bool Bb = (!this.IsValid(X, Y - 1) || this.CheckFor(X, Y - 1, Terrain.SOLID) || this.CheckFor(X, Y - 1, Terrain.HOLE));

								if (Bb || !this.IsValid(X - 1, Y) || this.CheckFor(X - 1, Y, Terrain.SOLID) || this.CheckFor(X - 1, Y, Terrain.HOLE)) {
									Array.Add(new Vector2(Xx, Yy));
								} else {
									Array.Add(new Vector2(Xx, Yy + 6));
									Array.Add(new Vector2(Xx + 6, Yy));
								}


								if (Bb || !this.IsValid(X - 1, Y) || this.CheckFor(X + 1, Y, Terrain.SOLID) || this.CheckFor(X + 1, Y, Terrain.HOLE)) {
									Array.Add(new Vector2(Xx + 16, Yy));
								} else {
									Array.Add(new Vector2(Xx + 16, Yy + 6));
									Array.Add(new Vector2(Xx + 10, Yy));
								}


								Array.Add(new Vector2(Xx, Yy + 16));
								Array.Add(new Vector2(Xx + 16, Yy + 16));
								Poly.Set(Array.ToArray({}));
							} else {
								List<Vector2> Array = new List<>();
								bool Bb = (!this.IsValid(X, Y - 1) || this.CheckFor(X, Y - 1, Terrain.SOLID) || this.CheckFor(X, Y - 1, Terrain.HOLE));

								if (Bb || !this.IsValid(X - 1, Y) || this.CheckFor(X - 1, Y, Terrain.SOLID) || this.CheckFor(X - 1, Y, Terrain.HOLE)) {
									Array.Add(new Vector2(Xx, Yy));
								} else {
									Array.Add(new Vector2(Xx, Yy + 6));
									Array.Add(new Vector2(Xx + 6, Yy));
								}


								if (Bb || !this.IsValid(X - 1, Y) || this.CheckFor(X + 1, Y, Terrain.SOLID) || this.CheckFor(X + 1, Y, Terrain.HOLE)) {
									Array.Add(new Vector2(Xx + 16, Yy));
								} else {
									Array.Add(new Vector2(Xx + 16, Yy + 6));
									Array.Add(new Vector2(Xx + 10, Yy));
								}


								if (this.CheckFor(X - 1, Y, Terrain.SOLID) || this.CheckFor(X - 1, Y, Terrain.HOLE)) {
									Array.Add(new Vector2(Xx, Yy + 12));
								} else {
									Array.Add(new Vector2(Xx, Yy + 6));
									Array.Add(new Vector2(Xx + 6, Yy + 12));
								}


								if (this.CheckFor(X + 1, Y, Terrain.SOLID) || this.CheckFor(X + 1, Y, Terrain.HOLE)) {
									Array.Add(new Vector2(Xx + 16, Yy + 12));
								} else {
									Array.Add(new Vector2(Xx + 10, Yy + 12));
									Array.Add(new Vector2(Xx + 16, Yy + 6));
								}


								Poly.Set(Array.ToArray({}));
							}


							FixtureDef Fixture = new FixtureDef();
							Fixture.Shape = Poly;
							Fixture.Friction = 0;
							Body.CreateFixture(Fixture);
							Poly.Dispose();
						} 
					} else if (this.CheckFor(X, Y, Terrain.HOLE)) {
						int Total = 0;

						foreach (Vector2 Vec in NEIGHBOURS8V) {
							Vector2 V = new Vector2(X + Vec.X, Y + Vec.Y);

							if (this.IsValid((int) V.X, (int) V.Y) && (this.CheckFor((int) V.X, (int) V.Y, Terrain.HOLE) || this.CheckFor((int) V.X, (int) V.Y, Terrain.SOLID))) {
								Total++;
							} 
						}

						if (Total < 8) {
							PolygonShape Poly = new PolygonShape();
							int Xx = X * 16;
							int Yy = Y * 16;

							if (this.CheckFor(X, Y + 1, Terrain.HOLE) || this.CheckFor(X, Y + 1, Terrain.SOLID)) {
								List<Vector2> Array = new List<>();
								bool Bb = (!this.IsValid(X, Y - 1) || this.CheckFor(X, Y - 1, Terrain.HOLE) || this.CheckFor(X, Y - 1, Terrain.SOLID));

								if (Bb || !this.IsValid(X - 1, Y) || this.CheckFor(X - 1, Y, Terrain.HOLE) || this.CheckFor(X - 1, Y, Terrain.SOLID)) {
									Array.Add(new Vector2(Xx, Yy));
								} else {
									Array.Add(new Vector2(Xx, Yy + 6));
									Array.Add(new Vector2(Xx + 6, Yy));
								}


								if (Bb || !this.IsValid(X - 1, Y) || this.CheckFor(X + 1, Y, Terrain.HOLE) || this.CheckFor(X + 1, Y, Terrain.SOLID)) {
									Array.Add(new Vector2(Xx + 16, Yy));
								} else {
									Array.Add(new Vector2(Xx + 16, Yy + 4));
									Array.Add(new Vector2(Xx + 10, Yy));
								}


								Array.Add(new Vector2(Xx, Yy + 16));
								Array.Add(new Vector2(Xx + 16, Yy + 16));
								Poly.Set(Array.ToArray({}));
							} else {
								List<Vector2> Array = new List<>();
								bool Bb = (!this.IsValid(X, Y - 1) || this.CheckFor(X, Y - 1, Terrain.HOLE) || this.CheckFor(X, Y - 1, Terrain.SOLID));

								if (Bb || !this.IsValid(X - 1, Y) || this.CheckFor(X - 1, Y, Terrain.HOLE) || this.CheckFor(X - 1, Y, Terrain.SOLID)) {
									Array.Add(new Vector2(Xx, Yy));
								} else {
									Array.Add(new Vector2(Xx, Yy + 4));
									Array.Add(new Vector2(Xx + 4, Yy));
								}


								if (Bb || !this.IsValid(X - 1, Y) || this.CheckFor(X + 1, Y, Terrain.HOLE) || this.CheckFor(X + 1, Y, Terrain.SOLID)) {
									Array.Add(new Vector2(Xx + 16, Yy));
								} else {
									Array.Add(new Vector2(Xx + 16, Yy + 4));
									Array.Add(new Vector2(Xx + 12, Yy));
								}


								if (this.CheckFor(X - 1, Y, Terrain.HOLE) || this.CheckFor(X - 1, Y, Terrain.SOLID)) {
									Array.Add(new Vector2(Xx, Yy + 8));
								} else {
									Array.Add(new Vector2(Xx, Yy + 4));
									Array.Add(new Vector2(Xx + 4, Yy + 8));
								}


								if (this.CheckFor(X + 1, Y, Terrain.HOLE) || this.CheckFor(X + 1, Y, Terrain.SOLID)) {
									Array.Add(new Vector2(Xx + 16, Yy + 8));
								} else {
									Array.Add(new Vector2(Xx + 12, Yy + 8));
									Array.Add(new Vector2(Xx + 16, Yy + 4));
								}


								Poly.Set(Array.ToArray({}));
							}


							FixtureDef Fixture = new FixtureDef();
							Fixture.Shape = Poly;
							Fixture.Friction = 0;
							Fixture.Filter.CategoryBits = 0x0002;
							Fixture.Filter.GroupIndex = -1;
							Fixture.Filter.MaskBits = -1;
							Chasms.CreateFixture(Fixture);
							Poly.Dispose();
						} 
					} 
				}
			}
		}

		public Void SetDecor(int X, int Y, byte V) {
			this.Decor[ToIndex(X, Y)] = V;
		}

		public byte GetVariants() {
			return this.Variants;
		}

		public bool Explored(int X, int Y) {
			return Explored[ToIndex(X, Y)];
		}

		public Room GetRandomRoom() {
			return this.Rooms.Get(Random.NewInt(this.Rooms.Size()));
		}

		public Room GetRandomRoom(Class Type) {
			for (int I = 0; I < 30; I++) {
				Room Room = this.Rooms.Get(Random.NewInt(this.Rooms.Size()));

				if (Type.IsInstance(Room)) {
					return Room;
				} 
			}

			return null;
		}

		public Point GetRandomFreePoint(Class Type) {
			for (int I = 0; I < 10; I++) {
				Room Room = this.GetRandomRoom(Type);

				if (Room == null || Room is EntranceRoom || Room is BossEntranceRoom) {
					continue;
				} 

				for (int J = 0; J < 100; J++) {
					Point Point = Room.GetRandomCell();
					int In = (int) (Point.X + Point.Y * GetWidth());

					if ((this.Passable == null || this.Passable[In]) && (this.Free == null || !this.Free[In])) {
						if (this.Free != null) {
							this.Free[In] = true;
						} 

						return Point;
					} 
				}
			}

			return null;
		}

		public List GetRooms<Room> () {
			return this.Rooms;
		}

		public Room FindRoomFor(float X, float Y) {
			Y += 4;

			foreach (Room Room in this.Rooms) {
				if (X > Room.Left * 16 + 8 && X < Room.Right * 16 + 8 && Y > Room.Top * 16 + 8 && Y < Room.Bottom * 16 + 8) {
					return Room;
				} 
			}

			return null;
		}

		public string GetMusic() {
			return "";
		}

		public override Void Save(FileWriter Writer) {
			Writer.WriteInt16((short) GetWidth());
			Writer.WriteInt16((short) GetHeight());
			int Sz = GetSize();

			for (int I = 0; I < Sz; I++) {
				Writer.WriteByte(this.Data[I]);
				Writer.WriteByte(this.LiquidData[I]);
				Writer.WriteByte(this.Decor[I]);
				Writer.WriteBoolean(this.Explored[I]);
				Writer.WriteInt32(this.Info[I]);
				short M = 1;

				while (M + I < Sz && this.Data[I + M] == this.Data[I] && this.LiquidData[I + M] == this.Data[I] && this.Decor[I + M] == this.Decor[I] && this.Explored[I + M] == this.Explored[I] && this.Info[I + M] == this.Info[I]) {
					M++;
				}

				Writer.WriteBoolean(M > 1);

				if (M > 1) {
					I += M - 1;
					Writer.WriteInt16((short) (M - 1));
				} 
			}

			Writer.WriteByte((byte) this.Rooms.Size());

			foreach (Room Room in this.Rooms) {
				Writer.WriteString(Room.GetClass().GetName());
				Writer.WriteInt16((short) Room.Left);
				Writer.WriteInt16((short) Room.Top);
				Writer.WriteInt16((short) Room.Right);
				Writer.WriteInt16((short) Room.Bottom);
				Writer.WriteBoolean(Room.Hidden);

				if (Room is EntranceRoom) {
					Writer.WriteBoolean(((EntranceRoom) Room).Exit);
				} 
			}

			int Count = 0;

			foreach (Room Room in this.Rooms) {
				foreach (Room N in Room.Neighbours) {
					int In = this.Rooms.IndexOf(N);

					if (In > -1) {
						Count++;
					} 
				}
			}

			Writer.WriteByte((byte) Count);

			for (int I = 0; I < this.Rooms.Size(); I++) {
				Room Room = this.Rooms.Get(I);

				foreach (Room N in Room.Neighbours) {
					int In = this.Rooms.IndexOf(N);

					if (In > -1) {
						Writer.WriteByte((byte) I);
						Writer.WriteByte((byte) In);
					} 
				}
			}
		}

		public override Void Load(FileReader Reader) {
			SetSize(Reader.ReadInt16(), Reader.ReadInt16());
			this.Data = new byte[GetSize()];
			this.Info = new int[GetSize()];
			this.LiquidData = new byte[GetSize()];
			this.Decor = new byte[GetSize()];
			this.WallDecor = new byte[GetSize()];
			this.Explored = new bool[GetSize()];
			this.InitLight();
			bool Vr = SaveManager.Version > 1;

			for (int I = 0; I < GetSize(); I++) {
				this.Data[I] = Reader.ReadByte();
				this.LiquidData[I] = Reader.ReadByte();
				this.Decor[I] = Reader.ReadByte();
				this.Explored[I] = Reader.ReadBoolean();
				this.Info[I] = Reader.ReadInt32();

				if (Vr && Reader.ReadBoolean()) {
					int J = I;
					short Cn = Reader.ReadInt16();

					for (int M = 0; M < Cn; M++) {
						I++;
						int In = J + M + 1;
						this.Data[In] = this.Data[J];
						this.LiquidData[In] = this.LiquidData[J];
						this.Decor[In] = this.Decor[J];
						this.Explored[In] = this.Explored[J];
						this.Info[In] = this.Info[J];
					}
				} 
			}

			try {
				int Count = Reader.ReadByte();
				this.Rooms = new List<>();

				for (int I = 0; I < Count; I++) {
					string T = Reader.ReadString();
					Class Clazz = Class.ForName(T);
					Object Object = Clazz.NewInstance();
					Room Room = (Room) Object;
					Room.Left = Reader.ReadInt16();
					Room.Top = Reader.ReadInt16();
					Room.Right = Reader.ReadInt16();
					Room.Bottom = Reader.ReadInt16();
					Room.Hidden = Reader.ReadBoolean();

					if (Room is EntranceRoom) {
						((EntranceRoom) Room).Exit = Reader.ReadBoolean();
					} 

					this.Rooms.Add(Room);
				}

				Count = Reader.ReadByte();

				for (int I = 0; I < Count; I++) {
					int In = Reader.ReadByte();
					Room Room = this.Rooms.Get(Reader.ReadByte());
					this.Rooms.Get(In).Connected.Put(Room, null);
					this.Rooms.Get(In).Neighbours.Add(Room);
				}
			} catch (Exception) {
				E.PrintStackTrace();
			}
		}
	}
}
