using BurningKnight.entity.creature.player;
using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.save;
using BurningKnight.game;
using BurningKnight.game.state;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight {
	public class Dungeon : ApplicationAdapter {
		public static ShaderProgram Shader;
		public static Game Game;
		public static int Depth;
		public static float Time;
		public static Level Level;
		public static Area Area;
		public static Area Ui;
		public static Dungeon Instance;
		public static bool Reset;
		public static byte LadderId;
		public static long LongTime;
		public static Entrance.LoadType LoadType = Entrance.LoadType.LOADING;
		public static Type Type = Type.REGULAR;
		public static float MAX_R = Math.Sqrt(Display.GAME_WIDTH * Display.GAME_WIDTH + Display.GAME_HEIGHT * Display.GAME_HEIGHT) / 2;
		public static float DarkR = MAX_R;
		public static float DarkX = Display.GAME_WIDTH / 2;
		public static float DarkY = Display.GAME_HEIGHT / 2;
		public static string[] Arg;
		public static float Speed = 1f;
		public static Color ORANGE = Color.ValueOf("#df7126");
		public static Color YELLOW = Color.ValueOf("#fbf236");
		public static int To = -10;
		private static Color Background = Color.BLACK;
		private static Color Background2 = Color.BLACK;
		public static float ShockTime = 10;
		public static float GlitchTime = 0;
		public static Vector2 ShockPos = new Vector2(0.5f, 0.5f);
		public static float Flip = -1;
		public static float ColorBlind = 0f;
		public static float ColorBlindFix = 1f;
		public static float Grayscale = 0f;
		public static float Dark = 1f;
		public static bool GoToMenu;
		public static float BattleDarkness;
		public static float White;
		public static bool GoToSelect;
		public static float Blood;
		public static Color FlashColor;
		public static float FlashTime;
		public static string Title;
		public static bool Quick;
		public static bool ToInventory;
		public static bool Steam = true;
		private static long StartTime = System.CurrentTimeMillis() / 1000;
		public static int LastDepth;
		public static float FpsY;
		public static float TimerY = 0;
		public static Cursor Cursor;
		private Vector2 Angle = new Vector2(0.0001f, 1.0f);
		private bool HadFocus = true;
		private Point InputVel = new Point();
		private float LastUpdate;
		private bool WasPaused;

		public static void Flash(Color Color, float Time) {
			FlashColor = Color;
			FlashTime = Time * Settings.Flash_frames * 2f;
		}

		public static void ReportException(Exception E) {
			Log.Report(E);
		}

		public static Color GetBackground() {
			return Background;
		}

		public static void SetBackground(Color Background) {
			Dungeon.Background = Background;
		}

		public static Color GetBackground2() {
			return Background2;
		}

		public static void SetBackground2(Color Background2) {
			Dungeon.Background2 = Background2;
		}

		public override void Resume() {
			base.Resume();

			if (Game.GetState() is InGameState && !Version.Debug) Game.GetState().SetPaused(WasPaused);
		}

		public override void Pause() {
			base.Pause();

			if (Game.GetState() is InGameState && !Version.Debug) {
				WasPaused = Game.GetState().IsPaused();
				Game.GetState().SetPaused(true);
			}
		}

		public static void NewGame() {
			NewGame(false, -1);
		}

		public static void BackToCastle(bool Quick, int Depth) {
			Reset = true;
			Dungeon.Quick = Quick;
			SaveManager.Delete();
			LoadType = Entrance.LoadType.GO_DOWN;
			Player.Instance = null;
			BurningKnight.Instance = null;
			UnlockClasses();
			Level = null;

			if (Area != null) Area.Destroy();

			Dungeon.Depth = Quick ? Depth : (Dungeon.Depth == -3 ? -3 : -2);

			if (Dungeon.Depth == -3) Quick = false;

			Game.SetState(new LoadState());
		}

		public static void UnlockClasses() {
			GameSave.RunId++;

			if (GameSave.RunId == 1) {
				Achievements.Unlock("CLASS_RANGED");
				GlobalSave.Put("unlocked_ranged", true);
			}
			else if (GameSave.RunId == 2) {
				Achievements.Unlock("CLASS_MELEE");
				GlobalSave.Put("unlocked_melee", true);
			}
		}

		public static void NewGame(bool Quick, int Depth) {
			Reset = true;
			Dungeon.Quick = Quick;
			SaveManager.Delete();
			GameSave.Time = 0;
			LoadType = Entrance.LoadType.GO_DOWN;
			UnlockClasses();
			Player.Instance = null;
			BurningKnight.Instance = null;
			Level = null;

			if (Area != null) Area.Destroy();

			Dungeon.Depth = Quick ? Depth : (Dungeon.Depth == -3 ? -3 : -2);

			if (Dungeon.Depth == -3) Quick = false;

			if (Quick && Dungeon.Depth != -1) {
				ItemSelectState.Depth = Dungeon.Depth;
				Game.SetState(new ItemSelectState());
			}
			else {
				Game.SetState(new LoadState());
			}
		}

		public static void GoToLevel(int Level) {
			To = Level;
		}

		public static void SlowDown(float A, float T) {
			Tween.To(new Tween.Task(A, 0.3f) {

		public override float GetValue() {
			return Speed;
		}

		public override void SetValue(float Value) {
			Speed = Value;
		}

		public override void OnEnd() {
			Tween.To(new Tween.Task(1f, T, Tween.Type.BACK_IN) {

		public override float GetValue() {
			return Speed;
		}

		public override void SetValue(float Value) {
			Speed = Value;
		}

		private enum Type {
			REGULAR,
			INTRO,
			ARCADE
		}
	});
}

});
}
private static void InitDiscord() {
}
private static void ShutdownDiscord() {
}
public static void BuildDiscordBadge() {
if (OS.Macos) {
return;
}
}
public override void Create() {
try {
if (!SteamAPI.Init()) {
Steam = false;
}
} catch (SteamException) {
E.PrintStackTrace();
Steam = false;
}
Instance = this;
if (Arg.Length > 0 && Arg[0].StartsWith("reset")) {
Dungeon.NewGame();
}
Log.Info("Burning knight " + Version.String);
Log.Info(new Date().ToString());
Log.Info("Loading from " + (Steam ? "Steam" : "native"));
InitDiscord();
LoadGlobal();
Achievements.Init();
Settings.Load();
long Seed = System.CurrentTimeMillis();
Camera Camera = new Camera();
Log.Info("Setting random seed to " + Seed + "...");
Random.Random.SetSeed(Seed);
Log.Info("Loading locale...");
Locale.Load("en");
this.SetupCursor();
Assets.Init();
string VertexShader;
string FragmentShader;
VertexShader = Gdx.Files.Internal("shaders/main.vert").ReadString();
FragmentShader = Gdx.Files.Internal("shaders/main.frag").ReadString();
Shader = new ShaderProgram(VertexShader, FragmentShader);
if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());
Box2D.Init();
this.InitColors();
this.InitInput();
Ui = new Area(true);
Area = new Area(true);
Game = new Game();
Game.SetState(new AssetLoadState());
Area.Add(Camera);
}
public override void Render() {
if (GoToMenu) {
GoToMenu = false;
Tween.To(new Tween.Task(0, 0.2f) {
public override float GetValue() {
return Dungeon.Dark;
}
public override void SetValue(float Value) {
Dungeon.Dark = Value;
}
public override void OnEnd() {
Game.SetState(new MainMenuState());
}
public override bool RunWhenPaused() {
return true;
}
});
}
if (GoToSelect) {
GoToSelect = false;
ItemSelectState.Depth = 1;
Game.SetState(new ItemSelectState());
}
if (AssetLoadState.Done && To > -10) {
Dungeon.LastDepth = Depth;
Dungeon.Depth = To;
Game.SetState(new LoadState());
To = -10;
}
Update();
Gdx.Gl.GlClearColor(GetBackground().R, GetBackground().G, GetBackground().B, 1);
Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT | (Gdx.Graphics.GetBufferFormat().CoverageSampling ? GL20.GL_COVERAGE_BUFFER_BIT_NV : 0));
RenderGame();
RenderUi();
if (Input.Instance != null) {
Input.Instance.Update();
}
}
private void Update() {
if (Graphics.DelayTime > 0) {
Graphics.DelayTime -= Gdx.Graphics.GetDeltaTime();
return;
}
float Dt = Math.Min(0.04f, Gdx.Graphics.GetDeltaTime()) * Speed;
Time += Dt;
LongTime += 1;
this.LastUpdate += Dt;
if (this.LastUpdate >= 0.06f) {
if (SteamAPI.IsSteamRunning()) {
SteamAPI.RunCallbacks();
}
this.LastUpdate = 0;
}
if (AssetLoadState.Done) {
Input.Instance.UpdateMousePosition();
}
Tween.Update(Dt);
ShockTime += Dt;
GlitchTime = Math.Max(0, GlitchTime - Dt);
if (Ui.Ui != null) {
Ui.Ui.Update(Dt);
}
if (Input.Instance.WasPressed("F2")) {
Tween.To(new Tween.Task(FpsY == 0 ? 18 : 0, 0.3f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return FpsY;
}
public override void SetValue(float Value) {
FpsY = Value;
}
public override bool RunWhenPaused() {
return true;
}
});
} else if (Input.Instance.WasPressed("F11")) {
Settings.Fullscreen = !Settings.Fullscreen;
if (Settings.Fullscreen) {
Gdx.Graphics.SetFullscreenMode(Gdx.Graphics.GetDisplayMode());
} else {
Gdx.Graphics.SetWindowedMode(Display.UI_WIDTH_MAX * 2, Display.UI_HEIGHT_MAX * 2);
}
} else if (Input.Instance.WasPressed("pause") && Dungeon.DarkR == Dungeon.MAX_R && Game.GetState() is InGameState && !Player.Instance.IsDead() && Dialog.Active == null && !Game.GetState().IsPaused()) {
Game.GetState().SetPaused(true);
Audio.PlaySfx("menu/select");
}
bool Paused = Game.GetState() != null && Game.GetState().IsPaused();
if (!(Game.GetState() is LoadState) && !Paused) {
Area.Update(Dt);
}
Dungeon.Ui.Update(Dt);
Achievements.Update(Dt);
Game.Update(Dt);
if (AssetLoadState.Done) {
UpdateMouse(Dt);
}
}
private void RenderGame() {
float Upscale = Math.Min(((float) Gdx.Graphics.GetWidth()) / Display.GAME_WIDTH, ((float) Gdx.Graphics.GetHeight()) / Display.GAME_HEIGHT) * Ui.Upscale;
Camera.ApplyShake();
float SceneX = Camera.Game.Position.X;
float SceneY = Camera.Game.Position.Y;
float SceneIX = MathUtils.Floor(SceneX);
float SceneIY = MathUtils.Floor(SceneY);
float UpscaleOffsetX = (SceneX - SceneIX) * Upscale;
float UpscaleOffsetY = (SceneY - SceneIY) * Upscale;
float SubpixelX = 0;
float SubpixelY = 0;
UpscaleOffsetX -= SubpixelX;
UpscaleOffsetY -= SubpixelY;
Camera.Game.Position.Set(SceneIX, SceneIY, 0);
Camera.Game.Update();
Graphics.Batch.SetProjectionMatrix(Camera.Game.Combined);
Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
if (Game.GetState() is InGameState && Level.SHADOWS) {
Graphics.Shadows.Begin();
Gdx.Gl.GlClearColor(0, 0, 0, 0);
Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT | (Gdx.Graphics.GetBufferFormat().CoverageSampling ? GL20.GL_COVERAGE_BUFFER_BIT_NV : 0));
Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
Graphics.Shape.SetColor(1, 1, 1, 1);
for (int I = 0; I < Area.GetEntities().Size(); I++) {
Entity Entity = Area.GetEntities().Get(I);
if (!Entity.IsActive()) {
continue;
}
if (Entity.OnScreen || Entity.AlwaysRender) {
Entity.RenderShadow();
}
}
Graphics.Shape.End();
Graphics.Shadows.End(Camera.Viewport.GetScreenX(), Camera.Viewport.GetScreenY(), Camera.Viewport.GetScreenWidth(), Camera.Viewport.GetScreenHeight());
}
Graphics.Surface.Begin();
Gdx.Gl.GlClearColor(GetBackground2().R, GetBackground2().G, GetBackground2().B, 1);
Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT);
Graphics.Batch.Begin();
if (!(Game.GetState() is LoadState)) {
Area.Render();
}
Game.Render(false);
Graphics.Batch.End();
Graphics.Surface.End();
Camera.RemoveShake();
Graphics.Batch.SetProjectionMatrix(Camera.ViewportCamera.Combined);
Texture Texture = Graphics.Surface.GetColorBufferTexture();
Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
Graphics.Batch.Begin();
Graphics.Batch.SetShader(Shader);
Shader.SetUniformf("u_textureSizes", Display.GAME_WIDTH, Display.GAME_HEIGHT, Upscale, 0.0f);
Shader.SetUniformf("u_sampleProperties", SubpixelX, SubpixelY, UpscaleOffsetX, UpscaleOffsetY);
Shader.SetUniformf("shockTime", ShockTime);
Shader.SetUniformf("glitchT", GlitchTime);
Shader.SetUniformf("shockPos", ShockPos);
Shader.SetUniformf("colorBlind", ColorBlind);
Shader.SetUniformf("correct", ColorBlindFix);
Shader.SetUniformf("grayscale", Grayscale);
Shader.SetUniformf("blood", Blood);
Shader.SetUniformf("ui", 0);
Shader.SetUniformf("white", White);
Shader.SetUniformf("battle", BattleDarkness);
Shader.SetUniformf("heat", 0);
Shader.SetUniformf("time", Dungeon.Time);
Shader.SetUniformf("transR", DarkR / MAX_R);
Shader.SetUniformf("dark", Dark);
Shader.SetUniformf("transPos", new Vector2(DarkX / Display.GAME_WIDTH, DarkY / Display.GAME_HEIGHT));
Shader.SetUniformf("cam", new Vector2(Camera.Game.Position.X / 1024f, Camera.Game.Position.Y / 1024f));
Graphics.Batch.SetColor(1, 1, 1, 1);
Graphics.Batch.SetProjectionMatrix(Graphics.Batch.GetProjectionMatrix().Rotate(0, 0, 1, Camera.Ma));
Graphics.Batch.Draw(Texture, -Display.GAME_WIDTH * Upscale / 2, -Flip * Display.GAME_HEIGHT * Upscale / 2, Display.GAME_WIDTH * Upscale, Flip * Display.GAME_HEIGHT * Upscale);
Graphics.Batch.SetProjectionMatrix(Graphics.Batch.GetProjectionMatrix().Rotate(0, 0, 1, -Camera.Ma));
Graphics.Batch.End();
Graphics.Batch.SetShader(null);
}
public static void TweenTimer(bool On) {
Tween.To(new Tween.Task(On ? 18 : 0, 0.3f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return TimerY;
}
public override void SetValue(float Value) {
TimerY = Value;
}
public override bool RunWhenPaused() {
return true;
}
});
}
public void RenderUi() {
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
Graphics.Shape.SetProjectionMatrix(Camera.Ui.Combined);
float Upscale = Math.Min(((float) Gdx.Graphics.GetWidth()) / Display.UI_WIDTH, ((float) Gdx.Graphics.GetHeight()) / Display.UI_HEIGHT);
Graphics.Shadows.Begin();
Gdx.Gl.GlClearColor(0, 0, 0, 0);
Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT);
Graphics.Batch.Begin();
Game.RenderUi();
if (FpsY > 0) {
int F = Gdx.Graphics.GetFramesPerSecond();
if (F >= 59) {
Graphics.Small.SetColor(0, 1, 0, 1);
} else if (F >= 49) {
Graphics.Small.SetColor(1, 0.5f, 0, 1);
} else {
Graphics.Small.SetColor(1, 0, 0, 1);
}
Graphics.Print(Integer.ToString(F), Graphics.Small, 2, Display.UI_HEIGHT - FpsY + 8);
Graphics.Small.SetColor(1, 1, 1, 1);
}
if (TimerY > 0 && !(Game.GetState() is MainMenuState)) {
string Time = string.Format("%02d:%02d:%02d:%02d", (int) Math.Floor(GameSave.Time / 3600), (int) Math.Floor(GameSave.Time / 60), (int) Math.Floor(GameSave.Time % 60), (int) Math.Floor(GameSave.Time % 1 * 100));
Graphics.Print(Time, Graphics.Small, 2 + FpsY, Display.UI_HEIGHT - TimerY + 8);
}
Graphics.Batch.End();
Graphics.Shadows.End();
Graphics.Batch.SetProjectionMatrix(Camera.ViewportCamera.Combined);
Texture Texture = Graphics.Shadows.GetColorBufferTexture();
Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
Graphics.Batch.Begin();
Graphics.Batch.SetShader(Shader);
if (FlashTime > 0) {
FlashTime -= Gdx.Graphics.GetDeltaTime();
Shader.SetUniformf("ft", 1);
Shader.SetUniformf("fr", FlashColor.R);
Shader.SetUniformf("fg", FlashColor.G);
Shader.SetUniformf("fb", FlashColor.B);
} else {
Shader.SetUniformf("ft", 0);
}
Shader.SetUniformf("u_sampleProperties", 0, 0, 0, 0);
Shader.SetUniformf("shockTime", 10);
Shader.SetUniformf("glitchT", 0);
Shader.SetUniformf("heat", 0);
Shader.SetUniformf("ui", 1);
Shader.SetUniformf("white", White);
Shader.SetUniformf("battle", 0);
Shader.SetUniformf("grayscale", 0);
Graphics.Batch.SetColor(1, 1, 1, 1);
Graphics.Batch.Draw(Texture, -Display.UI_WIDTH * Upscale / 2, Display.UI_HEIGHT * Upscale / 2, Display.UI_WIDTH * Upscale, -Display.UI_HEIGHT * Upscale);
Graphics.Batch.End();
Graphics.Batch.SetShader(null);
}
private void UpdateMouse(float Dt) {
if (Input.Instance.ActiveController == null) {
return;
}
InputVel.X -= InputVel.X * Dt * 10;
InputVel.Y -= InputVel.Y * Dt * 10;
float S = ((float) Gdx.Graphics.GetWidth()) / Display.GAME_WIDTH;
Vector2 Move = Input.Instance.GetAxis("cursor");
bool Big = Move.Len2() > 0.2;
if (Player.Instance != null) {
if (!Big) {
return;
}
Angle.Lerp(Move, 0.06f * Dt * 60);
float D = 48f;
Vector3 Input = Camera.Game.Project(new Vector3(Player.Instance.X + Player.Instance.W / 2 + Angle.X * D, Player.Instance.Y + Player.Instance.H / 2 + Angle.Y * D, 0));
Input.Instance.Mouse.X = Input.X;
Input.Instance.Mouse.Y = Gdx.Graphics.GetHeight() - Input.Y;
return;
}
if (Big) {
InputVel.X += Move.X * S;
InputVel.Y += Move.Y * S;
}
Input.Instance.Mouse.X += InputVel.X;
Input.Instance.Mouse.Y += InputVel.Y;
Input.Instance.Mouse.X = MathUtils.Clamp(0, Gdx.Graphics.GetWidth(), Input.Instance.Mouse.X);
Input.Instance.Mouse.Y = MathUtils.Clamp(0, Gdx.Graphics.GetHeight(), Input.Instance.Mouse.Y);
}
public override void Resize(int Width, int Height) {
Camera.Resize(Width, Height);
Input.Instance.Mouse.X = Gdx.Input.GetX();
Input.Instance.Mouse.Y = Gdx.Input.GetY();
State State = Game.GetState();
if (State != null) {
State.Resize(Width, Height);
}
Graphics.Resize(Width, Height);
}
public void SaveGlobal() {
SaveManager.Save(SaveManager.Type.GLOBAL, false);
}
public void LoadGlobal() {
try {
SaveManager.Load(SaveManager.Type.GLOBAL);
} catch (IOException) {
E.PrintStackTrace();
Log.Error("Failed to load global save, generating a new one");
SaveManager.Generate(SaveManager.Type.GLOBAL);
}
}
public override void Dispose() {
HandmadeRoom.Destroy();
if (Area != null) {
Ui.Destroy();
Area.Destroy();
}
Game.Destroy();
World.Destroy();
Assets.Destroy();
Settings.Save();
Achievements.Dispose();
SaveGlobal();
Log.Close();
if (Player.Shader != null) {
Player.Shader.Dispose();
Mob.Shader.Dispose();
Mob.Frozen.Dispose();
BurningKnight.Shader.Dispose();
Level.MaskShader.Dispose();
Level.Shader.Dispose();
WeaponBase.Shader.Dispose();
RectFx.Shader.Dispose();
Shader.Dispose();
InGameState.Shader.Dispose();
}
ShutdownDiscord();
SteamAPI.Shutdown();
}
private void InitInput() {
new Input();
}
private void SetupCursor() {
Pixmap Pm = new Pixmap(1, 1, Pixmap.Format.RGBA8888);
Pm.SetBlending(null);
Pm.SetColor(0, 0, 0, 0);
Cursor = Gdx.Graphics.NewCursor(Pm, 0, 0);
Gdx.Graphics.SetCursor(Cursor);
Pm.Dispose();
if (Settings.CursorId == Settings.Cursors.Length - 1) {
Gdx.Graphics.SetSystemCursor(Cursor.SystemCursor.Arrow);
}
}
private void InitColors() {
Colors.Put("black", Color.ValueOf("#000000"));
Colors.Put("gray", Color.ValueOf("#b4b4b4"));
Colors.Put("white", Color.ValueOf("#ffffff"));
Colors.Put("orange", Color.ValueOf("#ff5000"));
Colors.Put("red", Color.ValueOf("#ff0040"));
Colors.Put("green", Color.ValueOf("#5ac54f"));
Colors.Put("blue", Color.ValueOf("#0069aa"));
Colors.Put("cyan", Color.ValueOf("#00cdf9"));
Colors.Put("yellow", Color.ValueOf("#ffc825"));
Colors.Put("brown", Color.ValueOf("#8a4836"));
}
}
}