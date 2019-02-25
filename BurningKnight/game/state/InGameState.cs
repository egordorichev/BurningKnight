using BurningKnight.debug;
using BurningKnight.entity;
using BurningKnight.entity.level;
using BurningKnight.entity.level.blood;
using BurningKnight.entity.level.levels.desert;
using BurningKnight.entity.level.levels.forest;
using BurningKnight.entity.level.levels.ice;
using BurningKnight.entity.level.levels.library;
using BurningKnight.entity.level.levels.tech;
using BurningKnight.entity.level.save;
using BurningKnight.game.fx;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.game.state {
	public class InGameState : State {
		public static TextureRegion Noise;
		private static Music Fire;
		private static Music Water;
		public static string ToPlay;
		public static bool ResetMusic;
		private static long Hsfx = -1;
		public static bool Burning;
		public static bool Flow;
		public static bool Restart;
		public static bool StartTween;
		public static byte Id;
		public static bool NewGame;
		public static bool Portal;
		public static bool TriggerPause;
		public static bool Dark = true;
		public static ShaderProgram Shader;

		private Console Console;
		private string Depth;
		private float FlowVolume;
		private float Last;
		private int LastHp;
		private float LastSave;
		private Tween.Task LastTask;
		private Tween.Task LastTaskSize;
		private float Mv = -256;
		private bool Set;
		private bool SetFrames;
		private float Size;
		private float T;
		private float Time;
		private float Volume;
		private float W;
		private bool WasHidden;
		private bool Won;
		private float Ww;

		static InGameState() {
			Shader = new ShaderProgram(Gdx.Files.Internal("shaders/default.vert").ReadString(), Gdx.Files.Internal("shaders/fog.frag").ReadString());

			if (!Shader.IsCompiled()) throw new GdxRuntimeException("Couldn't compile shader: " + Shader.GetLog());
		}

		public override void Init() {
			if (Achievements.LastActive != null) Achievements.LastActive.Init();

			Ui.Ui.Reset();

			if (Fire == null) {
				Fire = Audio.GetMusic("OnFire");
				Water = Audio.GetMusic("water");
				Noise = new TextureRegion(new Texture(Gdx.Files.Internal("noise.png")));
			}

			SettingsX = 0;
			Ui.SaveAlpha = 0;
			Audio.Important = false;
			Dungeon.Dark = 1;
			Shader.Begin();
			var A = Random.NewFloat(Math.PI * 2);
			Shader.SetUniformf("tx", (float) Math.Cos(A));
			Shader.SetUniformf("ty", (float) Math.Sin(A));
			Shader.End();
			Dungeon.White = 0;
			Camera.Did = false;
			PauseMenuUi = new Area(true);
			var Collisions = new Collisions();
			World.World.SetContactListener(Collisions);
			World.World.SetContactFilter(Collisions);
			this.SetupUi();
			Dungeon.SetBackground2(new Color(Level.Colors[Dungeon.Level.Uid]));
			Console = new Console();

			if (Dungeon.Level is DesertLevel) {
				Achievements.Unlock(Achievements.REACH_DESERT);
				Achievements.Unlock(Achievements.UNLOCK_DEW_VIAL);
			}
			else if (Dungeon.Level is LibraryLevel) {
				Achievements.Unlock(Achievements.REACH_LIBRARY);
			}
			else if (Dungeon.Level is ForestLevel) {
				Achievements.Unlock(Achievements.REACH_FOREST);
			}
			else if (Dungeon.Level is BloodLevel) {
				Achievements.Unlock(Achievements.REACH_BLOOD);
			}
			else if (Dungeon.Level is IceLevel) {
				Achievements.Unlock(Achievements.REACH_ICE);
			}
			else if (Dungeon.Level is TechLevel) {
				Achievements.Unlock(Achievements.REACH_TECH);
			}

			if (BurningKnight.Instance == null && !GameSave.DefeatedBK && Dungeon.Depth > -1) {
				BurningKnight Knight = new BurningKnight();
				Dungeon.Area.Add(Knight);
				PlayerSave.Add(Knight);
			}

			for (var I = 0; I < 15; I++) Dungeon.Area.Add(new WindFx());

			Volume = 0;
			Dungeon.DarkR = Dungeon.MAX_R;
			PortalMod = 0;
			Tween.To(new Tween.Task(1, 1f) {

		public override float GetValue() {
			return PortalMod;
		}

		public override void SetValue(float Value) {
			PortalMod = Value;
		}

		public override bool RunWhenPaused() {
			return true;
		}

		public override void OnEnd() {
			base.OnEnd();
		}
	});
		if (ResetMusic) {
	ResetMusic = false;
	Audio.Reset();
	}
}

public override void SetPaused(bool Paused) {
base.SetPaused(Paused);
Dungeon.Dark = 1;
Dungeon.DarkR = Dungeon.MAX_R;
if (this.IsPaused()) {
if (!Player.Instance.IsDead()) {
if (Mv == 0) {
return;
}
this.Mv = -256;
Depth = Dungeon.Level.FormatDepth();
this.WasHidden = !UiMap.Instance.IsOpen();
Graphics.Layout.SetText(Graphics.Medium, Depth);
this.W = Graphics.Layout.Width;
Graphics.Layout.SetText(Graphics.Small, Random.GetSeed());
this.Ww = Graphics.Layout.Width;
if (!WasHidden) {
UiMap.Instance.Hide();
}
Tween.To(new Tween.Task(1, 0.3f) {
public override float GetValue() {
return Dungeon.Grayscale;
}
public override void SetValue(float Value) {
Dungeon.Grayscale = Value;
}
public override bool RunWhenPaused() {
return true;
}
});
Tween.To(new Tween.Task(0.2f * Settings.Music, 0.3f) {
public override float GetValue() {
return 1 * Settings.Music;
}
public override void SetValue(float Value) {
Audio.Current.SetVolume(Value);
}
public override bool RunWhenPaused() {
return true;
}
});
Tween.Remove(LastTask);
LastTask = Tween.To(new Tween.Task(0, 0.4f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return Mv;
}
public override void SetValue(float Value) {
Mv = Value;
}
public override bool RunWhenPaused() {
return true;
}
});
Tween.Remove(LastTaskSize);
LastTaskSize = Tween.To(new Tween.Task(52, 0.2f) {
public override float GetValue() {
return Size;
}
public override bool RunWhenPaused() {
return true;
}
public override void SetValue(float Value) {
Size = Value;
}
});
}
} else {
if (WasInSettings) {
WasInSettings = false;
SaveManager.SaveGames();
}
if (!WasHidden) {
UiMap.Instance.Show();
}
Tween.To(new Tween.Task(0, 0.3f) {
public override float GetValue() {
return Dungeon.Grayscale;
}
public override void SetValue(float Value) {
Dungeon.Grayscale = Value;
}
public override bool RunWhenPaused() {
return true;
}
});
Tween.To(new Tween.Task(1f * Settings.Music, 0.3f) {
public override float GetValue() {
return 0.5f * Settings.Music;
}
public override void SetValue(float Value) {
Audio.Current.SetVolume(Value);
}
public override bool RunWhenPaused() {
return true;
}
});
Tween.Remove(LastTaskSize);
LastTaskSize = Tween.To(new Tween.Task(0, 0.2f) {
public override float GetValue() {
return Size;
}
public override bool RunWhenPaused() {
return true;
}
public override void SetValue(float Value) {
Size = Value;
}
});
Tween.Remove(LastTask);
LastTask = Tween.To(new Tween.Task(-256, 0.2f) {
public override float GetValue() {
return Mv;
}
public override void SetValue(float Value) {
Mv = Value;
}
public override void OnEnd() {
base.OnEnd();
PauseMenuUi.Hide();
}
public override bool RunWhenPaused() {
return true;
}
});
}
}
public override void Destroy() {
base.Destroy();
SettingsX = 0;
Dungeon.Grayscale = 0;
Dungeon.White = 0;
Ui.SaveAlpha = 0;
this.Console.Destroy();
Dungeon.BattleDarkness = 0;
Camera.Instance.ResetShake();
bool Old = (Dungeon.Game.GetState() is LoadState);
if (Player.Instance != null && !Player.Instance.IsDead()) {
SaveManager.Save(SaveManager.Type.GAME, Old);
SaveManager.Save(SaveManager.Type.LEVEL, Old);
SaveManager.Save(SaveManager.Type.PLAYER, Old);
}
if (Dungeon.Area != null) {
Dungeon.Area.Destroy();
}
PauseMenuUi.Destroy();
Fire.SetVolume(0);
Fire.Pause();
Water.SetVolume(0);
Water.Pause();
}
public static void Horn() {
if (Settings.Sfx == 0) {
return;
}
Sound Sound = Audio.GetSound("airhorn");
Sound.Stop(Hsfx);
Camera.Shake(5);
Hsfx = Sound.Play(Settings.Sfx);
}
private void LightUp(Room Room) {
for (int X = Room.Left; X <= Room.Right; X++) {
for (int Y = Room.Top; Y <= Room.Bottom; Y++) {
if ((X == Room.Left || X == Room.Right || Y == Room.Top || Y == Room.Bottom) && (Dungeon.Level.CheckFor(X, Y, Terrain.PASSABLE) || Dungeon.Level.CheckFor(X, Y, Terrain.HOLE))) {
Dungeon.Level.AddLightInRadius(X * 16, Y * 16, 2f, 3f, false);
}
if (Y != Room.Top) {
Dungeon.Level.AddLight(X * 16, Y * 16, 4f, 4f);
}
}
}
}
public override void Update(float Dt) {
T += Dt;
if (T >= 0.1f && TriggerPause && !IsPaused()) {
TriggerPause = false;
SetPaused(true);
}
Dungeon.SetBackground2((Level.Colors[Dungeon.Level.Uid]));
UiInventory.JustUsed = Math.Max(0, UiInventory.JustUsed - 1);
if (Dungeon.Depth == -2) {
Upgrade.UpdateEvent = false;
}
if (StartTween) {
Audio.Play("Void");
StartTween = false;
FromCenter = true;
Tween.To(new Tween.Task(0, 1f) {
public override float GetValue() {
return PortalMod;
}
public override bool RunWhenPaused() {
return true;
}
public override void SetValue(float Value) {
PortalMod = Value;
}
public override void OnEnd() {
if (NewGame) {
NewGame = false;
Dungeon.NewGame(true, 1);
} else if (Restart) {
Restart = false;
Dungeon.Grayscale = 0;
if (Dungeon.Depth == -3) {
Dungeon.NewGame(false, -3);
} else {
Dungeon.NewGame(true, 1);
}
} else if (Portal) {
Portal = false;
if (Dungeon.Depth == -2) {
Dungeon.GoToSelect = true;
} else {
Dungeon.GoToLevel(Dungeon.Depth + 1);
Player.Instance.Rotating = false;
Dungeon.LoadType = Entrance.LoadType.GO_DOWN;
}
Camera.NoMove = false;
Dungeon.SetBackground2(new Color(0, 0, 0, 1));
Player.Sucked = false;
} else {
if (Dungeon.Depth == -2) {
Dungeon.GoToSelect = true;
} else {
GameSave.Inventory = true;
Dungeon.ToInventory = true;
Dungeon.LoadType = Entrance.LoadType.GO_DOWN;
Dungeon.LadderId = Id;
}
}
}
});
}
if (!IsPaused() && !Player.Instance.IsDead() && Dungeon.Depth != -2) {
GameSave.Time += Gdx.Graphics.GetDeltaTime();
} else {
Dungeon.Speed += (1 - Dungeon.Speed) * Dt * 5;
}
if (IsPaused()) {
Camera.Instance.Update(Dt);
} else {
this.Time += Dt;
this.LastSave += Dt;
if (LastSave >= 180f) {
LastSave = 0;
SaveManager.SaveGame();
}
}
Orbital.UpdateTime(Dt);
if (Player.Instance.Room != null) {
LightUp(Player.Instance.Room);
}
foreach (Room Room in Dungeon.Level.GetRooms()) {
if (Room is BossEntranceRoom) {
LightUp(Room);
break;
}
}
if (Input.Instance.WasPressed("F")) {
Horn();
if (!Won) {
Won = true;
Ui.Ui.OnWin();
}
}
if (Version.Debug) {
this.Console.Update(Dt);
if (Input.Instance.WasPressed("F4")) {
Dungeon.DarkR = Dungeon.MAX_R;
Player.Instance.SetUnhittable(true);
Camera.Follow(null);
Vector3 Vec = Camera.Game.Project(new Vector3(Player.Instance.X + Player.Instance.W / 2, Player.Instance.Y + Player.Instance.H / 2, 0));
Vec = Camera.Ui.Unproject(Vec);
Vec.Y = Display.GAME_HEIGHT - Vec.Y / Display.UI_SCALE;
Dungeon.DarkX = Vec.X / Display.UI_SCALE;
Dungeon.DarkY = Vec.Y;
Tween.To(new Tween.Task(0, 0.3f, Tween.Type.QUAD_OUT) {
public override float GetValue() {
return Dungeon.DarkR;
}
public override void SetValue(float Value) {
Dungeon.DarkR = Value;
}
public override void OnEnd() {
int Level = Dungeon.Depth;
Dungeon.NewGame(true, Level);
Dungeon.SetBackground2(new Color(0, 0, 0, 1));
}
});
}
}
if (Version.Debug) {
if (Input.Instance.WasPressed("F5")) {
foreach (Room Room in Dungeon.Level.GetRooms()) {
if (Room is SecretRoom) {
BombEntity.Make(Room);
Dungeon.Level.LoadPassable();
Dungeon.Level.AddPhysics();
Point Point = Room.GetRandomCell();
if (Point != null) {
Player.Instance.Tp(Point.X * 16, Point.Y * 16);
}
break;
}
}
} else if (Input.Instance.WasPressed("F6")) {
foreach (Room Room in Dungeon.Level.GetRooms()) {
if (Room is TreasureRoom) {
Point Point = Room.GetRandomFreeCell();
Player.Instance.Tp(Point.X * 16, Point.Y * 16);
break;
}
}
} else if (Input.Instance.WasPressed("F7")) {
foreach (Room Room in Dungeon.Level.GetRooms()) {
if (Room is BossRoom && Room != Player.Instance.Room) {
Point Point = Room.GetRandomFreeCell();
if (Point != null) {
Player.Instance.Tp(Point.X * 16, Point.Y * 16);
}
break;
}
}
} else if (Input.Instance.WasPressed("F8")) {
foreach (Room Room in Dungeon.Level.GetRooms()) {
if (Room is NpcSaveRoom && Room != Player.Instance.Room) {
Point Point = Room.GetRandomFreeCell();
if (Point != null) {
Player.Instance.Tp(Point.X * 16, Point.Y * 16);
}
break;
}
}
}
if (Input.Instance.WasPressed("F3")) {
Ui.HideUi = !Ui.HideUi;
} else if (Input.Instance.WasPressed("F9")) {
Ui.HideCursor = !Ui.HideCursor;
}
if (Input.Instance.WasPressed("O")) {
Ui.Upscale = 1;
} else if (Input.Instance.IsDown("I")) {
Ui.Upscale = Math.Max(0.1f, Ui.Upscale - Dt * 3);
} else if (Input.Instance.IsDown("P")) {
Ui.Upscale += Dt * 3;
}
}
if (Player.Instance != null && !Player.Instance.IsDead()) {
Last += Dt;
if (Last >= 1f) {
Last = 0;
if (PortalMod == 1) {
CheckMusic();
}
}
}
if (this.IsPaused()) {
PauseMenuUi.Update(Dt);
} else {
World.Update(Dt);
}
if (Dialog.Active != null) {
Dialog.Active.Update(Dt);
}
if (Player.Instance != null && (Player.Instance.GetHp() < LastHp || Player.DullDamage)) {
if (!SetFrames) {
Player.DullDamage = false;
SetFrames = true;
Tween.To(new Tween.Task(0.1f, 0.1f) {
public override float GetValue() {
return Dungeon.Blood;
}
public override void SetValue(float Value) {
Dungeon.Blood = Value;
}
public override void OnEnd() {
if (Player.Instance.GetHp() + Player.Instance.GetGoldenHearts() + Player.Instance.GetIronHearts() > 1) {
Tween.To(new Tween.Task(0, 0.4f) {
public override float GetValue() {
return Dungeon.Blood;
}
public override void SetValue(float Value) {
Dungeon.Blood = Value;
}
});
}
}
});
}
} else {
SetFrames = false;
}
if (Player.Instance != null) {
LastHp = Player.Instance.GetHp();
}
Dark = Player.Instance.IsDead();
if (!Dark) {
Dark = Boss.All.Size() > 0 && Player.Instance.Room is BossRoom && !BurningKnight.Instance.Rage;
if (!Dark) {
foreach (Mob Mob in Mob.All) {
if (Mob.Room == Player.Instance.Room) {
Dark = true;
break;
}
}
}
}
Dungeon.BattleDarkness = 0;
bool None = Volume <= 0.05f;
Volume += ((Burning ? 1 : 0) - Volume) * Dt;
try {
if (Volume > 0.05f && None) {
Fire.Play();
} else if (Volume < 0.05f && !None) {
Fire.Pause();
}
Fire.SetVolume(Volume * Settings.Sfx);
Burning = false;
None = Volume <= 0.05f;
FlowVolume += ((Flow ? 1 : 0) - FlowVolume) * Dt;
if (FlowVolume > 0.05f && None) {
Water.Play();
} else if (FlowVolume < 0.05f && !None) {
Water.Pause();
}
Water.SetVolume(FlowVolume * Settings.Sfx * 0.25f);
} catch (GdxRuntimeException) {
}
Flow = false;
}
public static void CheckMusic() {
if (Dungeon.Game.GetState() is InGameState) {
if (Dungeon.Depth == -2 || Player.Instance.Room is ShopRoom) {
Audio.Play("Shopkeeper");
} else if (Player.Instance.Room is PrebossRoom) {
Audio.Play("Gobbeon");
} else if (Player.Instance.Room is SecretRoom) {
Audio.Play("Serendipity");
} else if ((BurningKnight.Instance == null || !(BurningKnight.Instance.Dest))) {
Audio.Play(Dungeon.Level.GetMusic());
}
}
}
public override void Render() {
base.Render();
if (Dungeon.Depth > -3 && Settings.Quality > 1) {
Graphics.Batch.End();
Graphics.Batch.SetShader(Shader);
Shader.Begin();
Shader.SetUniformf("time", Time * 0.01f);
Shader.SetUniformf("cx", Camera.Game.Position.X / 512);
Shader.SetUniformf("cy", -Camera.Game.Position.Y / 512);
Shader.End();
Graphics.Batch.Begin();
Graphics.Render(Noise, Camera.Game.Position.X - Display.GAME_WIDTH / 2, Camera.Game.Position.Y - Display.GAME_HEIGHT / 2, 0, 0, 0, false, false);
Graphics.Batch.End();
Graphics.Batch.SetShader(null);
Graphics.Batch.Begin();
if (Dungeon.Depth == -2) {
foreach (Upgrade Upgrade in Upgrade.All) {
Upgrade.RenderSigns();
}
}
}
Player.Instance.RenderBuffs();
}
public override void RenderUi() {
Dungeon.Ui.Render();
Ui.Ui.Render();
if (this.Size > 0) {
Graphics.StartShape();
Graphics.Shape.SetColor(0, 0, 0, 1);
Graphics.Shape.Rect(0, 0, Display.UI_WIDTH, Size);
Graphics.Shape.Rect(0, Display.UI_HEIGHT - Size, Display.UI_WIDTH, Size);
Graphics.EndShape();
}
Graphics.Batch.SetProjectionMatrix(Camera.Game.Combined);
World.Render();
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
this.Console.Render();
if (Dialog.Active != null) {
Dialog.Active.Render();
}
if (this.Mv > -256) {
Camera.Ui.Translate(SettingsX, this.Mv);
Camera.Ui.Update();
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
PauseMenuUi.Render();
Graphics.Print(this.Depth, Graphics.Medium, Display.UI_WIDTH / 2 - W / 2, 128 + 32 + 16);
Graphics.Print(Random.GetSeed(), Graphics.Small, Display.UI_WIDTH / 2 - Ww / 2, 128 + 32 + 12);
Camera.Ui.Translate(-SettingsX, -this.Mv);
Camera.Ui.Update();
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
}
if (PortalMod < 1) {
RenderPortalOpen();
}
Achievements.Render();
Ui.Ui.RenderCursor();
Ui.RenderSaveIcon(1);
}
private void SetupUi() {
UiInventory Inventory = new UiInventory(Player.Instance.GetInventory());
Dungeon.Ui.Add(Inventory);
Dungeon.Ui.Add(new UiMap());
int Y = -24 + 16;
int M = 0;
if (Dungeon.Depth == -2) {
M = 1;
}
SettingsFirst = (UiButton) this.PauseMenuUi.Add(new UiButton("resume", Display.UI_WIDTH / 2, 128 + 32 + Y - M * 24) {
public override void OnClick() {
Audio.PlaySfx("menu/exit");
SetPaused(false);
}
public override void Render() {
base.Render();
if (SettingsX == 0 && Input.Instance.WasPressed("pause")) {
Input.Instance.PutState("pause", Input.State.UP);
this.OnClick();
}
}
}.SetSparks(true));
if (Dungeon.Depth != -2) {
M++;
this.PauseMenuUi.Add(new UiButton("quick_restart", Display.UI_WIDTH / 2, 128 + 32 - 24 + Y) {
public override void OnClick() {
base.OnClick();
if (!Restart) {
StartTween = true;
Restart = true;
}
}
}.SetSparks(true));
}
this.PauseMenuUi.Add(new UiButton("settings", Display.UI_WIDTH / 2, 128 + 32 - 24 * (M + 1) + Y) {
public override void OnClick() {
base.OnClick();
AddSettings();
Tween.To(new Tween.Task(Display.UI_WIDTH, 0.15f, Tween.Type.QUAD_IN_OUT) {
public override float GetValue() {
return SettingsX;
}
public override void SetValue(float Value) {
SettingsX = Value;
}
public override bool RunWhenPaused() {
return true;
}
});
}
}.SetSparks(true));
this.PauseMenuUi.Add(new UiButton("save_and_exit", Display.UI_WIDTH / 2, 128 + 32 - 24 * (M + 2) + Y) {
public override void OnClick() {
Audio.PlaySfx("menu/exit");
Tween.To(new Tween.Task(0f * Settings.Music, 0.3f) {
public override float GetValue() {
return 0.5f * Settings.Music;
}
public override void SetValue(float Value) {
Audio.Current.SetVolume(Value);
}
public override bool RunWhenPaused() {
return true;
}
});
Transition(new Runnable() {
public override void Run() {
Dungeon.Grayscale = 0;
Dungeon.Game.SetState(new MainMenuState());
}
});
}
});
foreach (Entity Entity in this.PauseMenuUi.GetEntities()) {
Entity.SetActive(false);
}
}
public override void OnPause() {
base.OnPause();
if (!Player.Instance.IsDead()) {
this.PauseMenuUi.Show();
this.PauseMenuUi.SelectFirst();
}
}
}
}