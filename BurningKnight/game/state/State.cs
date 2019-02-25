using BurningKnight.entity;
using BurningKnight.ui;
using BurningKnight.util;

namespace BurningKnight.game.state {
	public class State {
		public static TextureRegion Player;
		public static float SettingsX;
		public bool AddedSettings;
		private bool Crazy;
		private List<Entity> CurrentSettings = new List<>();
		protected bool FromCenter;
		protected UiButton GraphicsButton;
		private bool Paused;
		protected Area PauseMenuUi;
		protected float PortalMod;
		protected UiButton SettingsFirst;
		protected bool WasInSettings;

		public State() {
			Crazy = Random.GetSeed().Equals("MAANEX");
		}

		protected void RenderPortalOpen() {
			if (Player == null) Player = Graphics.GetTexture("props-gobbo_full");

			Graphics.StartAlphaShape();
			Graphics.Shape.SetProjectionMatrix(Camera.Nil.Combined);
			Color Cl = ColorUtils.HSV_to_RGB(Dungeon.Time * (Crazy ? 60 : 20) % 360, 360, 360);
			Dungeon.SetBackground2(new Color(Cl.R * 0.04f, Cl.G * 0.04f, Cl.B * 0.04f, 1f));
			float Vv = Math.Max(0, 1 - PortalMod);

			for (var I = 0; I < 65; I++) {
				var S = I * 0.015f;
				var Mx = Noise.Instance.Noise(Dungeon.Time * 0.25f + S) * 96 * Vv;
				var My = Noise.Instance.Noise(3 + Dungeon.Time * 0.25f + S) * 96 * Vv;
				var V = I / 65f;
				Color Color = ColorUtils.HSV_to_RGB((Dungeon.Time * (Crazy ? 60 : 20) - I * 1.4f) % 360, 360, 360);
				Graphics.Shape.SetColor(V * Color.R, V * Color.G, V * Color.B, 1f);
				var A = Math.PI * I * 0.2f + Dungeon.Time * (Crazy ? 6 : 2);
				var C = FromCenter ? -194 * PortalMod : PortalMod * Display.UI_WIDTH;
				var W = I * 2 + 64 + C;

				if (W <= 0) continue;

				var D = I * 2.5f * (I * 0.01f + 0.99f);

				if (FromCenter)
					D = D * (1 - PortalMod);
				else
					D += C;


				if (D <= 0) continue;

				var X = Math.Cos(A) * D + Display.GAME_WIDTH / 2 + Mx * (((float) 56 - I) / 56);
				var Y = Math.Sin(A) * D + Display.GAME_HEIGHT / 2 + My * (((float) 56 - I) / 56);
				Graphics.Shape.Rect(X - W / 2, Y - W / 2, W / 2, W / 2, W, W, 1f, 1f, (float) Math.ToDegrees(A + 0.1f));
			}

			Graphics.EndAlphaShape();
			Graphics.Shape.SetProjectionMatrix(Camera.Ui.Combined);
		}

		protected void RenderPortal() {
			if (Player == null) Player = Graphics.GetTexture("props-gobbo_full");

			Graphics.StartAlphaShape();
			Graphics.Shape.SetProjectionMatrix(Camera.Nil.Combined);
			Color Cl = ColorUtils.HSV_to_RGB(Dungeon.Time * (Crazy ? 60 : 20) % 360, 360, 360);
			Dungeon.SetBackground2(new Color(Cl.R * 0.04f, Cl.G * 0.04f, Cl.B * 0.04f, 1f));

			for (var I = 0; I < 65; I++) {
				var S = I * 0.015f;
				float Mx = Noise.Instance.Noise(Dungeon.Time * 0.25f + S) * 96;
				float My = Noise.Instance.Noise(3 + Dungeon.Time * 0.25f + S) * 96;
				var V = I / 65f;
				Color Color = ColorUtils.HSV_to_RGB((Dungeon.Time * (Crazy ? 60 : 20) - I * 1.4f) % 360, 360, 360);
				Graphics.Shape.SetColor(V * Color.R, V * Color.G, V * Color.B, 1f);
				var A = Math.PI * I * 0.2f + Dungeon.Time * (Crazy ? 6 : 2);
				float W = I * 2 + 64;
				var D = I * 2.5f * (I * 0.01f + 0.99f);
				var X = Math.Cos(A) * D + Display.GAME_WIDTH / 2 + Mx * (((float) 56 - I) / 56);
				var Y = Math.Sin(A) * D + Display.GAME_HEIGHT / 2 + My * (((float) 56 - I) / 56);
				Graphics.Shape.Rect(X - W / 2, Y - W / 2, W / 2, W / 2, W, W, 1f, 1f, (float) Math.ToDegrees(A + 0.1f));
			}

			Graphics.EndAlphaShape();
			Graphics.Shape.SetProjectionMatrix(Camera.Ui.Combined);
		}

		public void OnPause() {
		}

		public void OnUnpause() {
		}

		public bool IsPaused() {
			return Paused;
		}

		public void SetPaused(bool Paused) {
			this.Paused = Paused;

			if (this.Paused)
				OnPause();
			else
				OnUnpause();
		}

		public void Init() {
		}

		public void Destroy() {
		}

		public void Update(float Dt) {
		}

		public void Render() {
		}

		public void RenderUi() {
		}

		public void Resize(int Width, int Height) {
		}

		public static void Transition(Runnable Runnable) {
			Tween.To(new Tween.Task(0, 0.2f) {

		public override float GetValue() {
			return Dungeon.Dark;
		}

		public override void SetValue(float Value) {
			Dungeon.Dark = Value;
		}

		public override void OnEnd() {
			Runnable.Run();
			Tween.To(new Tween.Task(1, 0.2f) {

		public override float GetValue() {
			return Dungeon.Dark;
		}

		public override void SetValue(float Value) {
			Dungeon.Dark = Value;
		}
	});
}

public override bool RunWhenPaused() {
return true;
}
});
}
public void AddSettings() {
if (AddedSettings) {
return;
}
AddedSettings = true;
float S = 20;
float St = 60 + 20f;
this.PauseMenuUi.Add(new UiButton("back", (int) (Display.UI_WIDTH * 1.5f), (int) St) {
public override void Render() {
base.Render();
if (SettingsX == Display.UI_WIDTH && Input.Instance.WasPressed("pause")) {
Input.Instance.PutState("pause", Input.State.UP);
this.OnClick();
}
}
public override void OnClick() {
Audio.PlaySfx("menu/exit");
PauseMenuUi.Select(SettingsFirst);
Tween.To(new Tween.Task(0, 0.15f, Tween.Type.QUAD_IN_OUT) {
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
});
this.PauseMenuUi.Add(new UiButton("audio", (int) (Display.UI_WIDTH * 1.5f), (int) (St + S * 2)) {
public override void OnClick() {
base.OnClick();
AddAudio();
Tween.To(new Tween.Task(Display.UI_WIDTH * 2f, 0.15f, Tween.Type.QUAD_IN_OUT) {
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
});
this.PauseMenuUi.Add(new UiButton("controls", (int) (Display.UI_WIDTH * 1.5f), (int) (St + S * 3)) {
public override void OnClick() {
base.OnClick();
AddControls();
Tween.To(new Tween.Task(Display.UI_WIDTH * 2f, 0.15f, Tween.Type.QUAD_IN_OUT) {
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
});
this.PauseMenuUi.Add(new UiButton("game", (int) (Display.UI_WIDTH * 1.5f), (int) (St + S * 4)) {
public override void OnClick() {
base.OnClick();
AddGame();
Tween.To(new Tween.Task(Display.UI_WIDTH * 2f, 0.15f, Tween.Type.QUAD_IN_OUT) {
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
});
GraphicsButton = (UiButton) this.PauseMenuUi.Add(new UiButton("graphics", (int) (Display.UI_WIDTH * 1.5f), (int) (St + S * 5)) {
public override void OnClick() {
base.OnClick();
AddGraphics();
Tween.To(new Tween.Task(Display.UI_WIDTH * 2f, 0.15f, Tween.Type.QUAD_IN_OUT) {
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
});
SelectGraphics();
}
private void SelectGraphics() {
PauseMenuUi.Select(GraphicsButton);
}
public void Clear() {
WasInSettings = true;
UiChoice.MaxW = 0;
foreach (Entity E in CurrentSettings) {
E.Done = true;
}
}
public void AddControls() {
Clear();
float S = 20;
float St = 60 + 5f;
CurrentSettings.Add(PauseMenuUi.Add(new UiButton("back", (int) (Display.UI_WIDTH * 2.5f), (int) (St)) {
public override void Render() {
base.Render();
if (SettingsX == Display.UI_WIDTH * 2 && Input.Instance.WasPressed("pause")) {
Input.Instance.PutState("pause", Input.State.UP);
this.OnClick();
}
}
public override void OnClick() {
Audio.PlaySfx("menu/exit");
SelectGraphics();
Tween.To(new Tween.Task(Display.UI_WIDTH * 1f, 0.15f, Tween.Type.QUAD_IN_OUT) {
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
}));
UiEntity Button = (UiEntity) PauseMenuUi.Add(new UiKey("use", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 6)));
CurrentSettings.Add(Button);
CurrentSettings.Add(PauseMenuUi.Add(new UiKey("switch", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 5))));
CurrentSettings.Add(PauseMenuUi.Add(new UiKey("interact", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 4))));
CurrentSettings.Add(PauseMenuUi.Add(new UiKey("active", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 3))));
CurrentSettings.Add(PauseMenuUi.Add(new UiKey("roll", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 2))));
PauseMenuUi.Select(Button);
}
public void AddGraphics() {
Clear();
float S = 13;
float St = 60 - 5f;
CurrentSettings.Add(PauseMenuUi.Add(new UiButton("back", (int) (Display.UI_WIDTH * 2.5f), (int) (St)) {
public override void Render() {
base.Render();
if (SettingsX == Display.UI_WIDTH * 2 && Input.Instance.WasPressed("pause")) {
Input.Instance.PutState("pause", Input.State.UP);
this.OnClick();
}
}
public override void OnClick() {
Audio.PlaySfx("menu/exit");
SelectGraphics();
Tween.To(new Tween.Task(Display.UI_WIDTH * 1f, 0.15f, Tween.Type.QUAD_IN_OUT) {
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
}));
CurrentSettings.Add(PauseMenuUi.Add(new UiCheckbox("rotate_cursor", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 4)) {
public override void OnClick() {
base.OnClick();
Settings.RotateCursor = this.IsOn();
}
}.SetOn(Settings.RotateCursor)));
CurrentSettings.Add(PauseMenuUi.Add(new UiChoice("cursor", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 5)) {
public override void OnClick() {
base.OnClick();
Settings.Cursor = Settings.Cursors[this.GetCurrent()];
Settings.CursorId = this.GetCurrent();
if (Settings.CursorId == Settings.Cursors.Length - 1) {
Gdx.Graphics.SetSystemCursor(Cursor.SystemCursor.Arrow);
} else {
Gdx.Graphics.SetCursor(Dungeon.Cursor);
}
}
}.SetChoices({ "1/9", "2/9", "3/9", "4/9", "5/9", "6/9", "7/9", "8/9", "native" }).SetCurrent(Settings.GetCursorId(Settings.Cursor))));
CurrentSettings.Add(PauseMenuUi.Add(new UiChoice("colorblind_mode", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 6)) {
public override void OnUpdate() {
Dungeon.ColorBlind = GetCurrent();
}
}.SetChoices({ "none", "protanope", "deuteranope", "tritanope" }).SetCurrent((int) Dungeon.ColorBlind)));
CurrentSettings.Add(PauseMenuUi.Add(new UiSlider("freeze_frames", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 7)) {
public override void OnUpdate() {
Settings.Freeze_frames = this.Val;
}
}.SetValue(Settings.Freeze_frames)));
CurrentSettings.Add(PauseMenuUi.Add(new UiSlider("flash_frames", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 8)) {
public override void OnUpdate() {
Settings.Flash_frames = this.Val;
}
}.SetValue(Settings.Flash_frames)));
CurrentSettings.Add(PauseMenuUi.Add(new UiSlider("screenshake", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 9)) {
public override void OnUpdate() {
Settings.Screenshake = this.Val;
}
}.SetValue(Settings.Screenshake)));
CurrentSettings.Add(PauseMenuUi.Add(new UiChoice("quality", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 10)) {
public override void OnUpdate() {
Settings.Quality = GetCurrent();
}
}.SetChoices({ "bad", "good", "great" }).SetCurrent(Settings.Quality)));
UiEntity Button = (UiEntity) PauseMenuUi.Add(new UiCheckbox("fullscreen", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 11)) {
public override void OnClick() {
base.OnClick();
Settings.Fullscreen = this.IsOn();
if (Settings.Fullscreen) {
Gdx.Graphics.SetFullscreenMode(Gdx.Graphics.GetDisplayMode());
} else {
Gdx.Graphics.SetWindowedMode(Display.UI_WIDTH_MAX * 2, Display.UI_HEIGHT_MAX * 2);
}
}
public override void Render() {
SetOn(Settings.Fullscreen);
base.Render();
}
}.SetOn(Settings.Fullscreen));
CurrentSettings.Add(Button);
PauseMenuUi.Select(Button);
}
public void AddAudio() {
Clear();
float S = 20;
float St = 60 + 20f;
CurrentSettings.Add(PauseMenuUi.Add(new UiButton("back", (int) (Display.UI_WIDTH * 2.5f), (int) (St)) {
public override void Render() {
base.Render();
if (SettingsX == Display.UI_WIDTH * 2 && Input.Instance.WasPressed("pause")) {
Input.Instance.PutState("pause", Input.State.UP);
this.OnClick();
}
}
public override void OnClick() {
Audio.PlaySfx("menu/exit");
SelectGraphics();
Tween.To(new Tween.Task(Display.UI_WIDTH * 1f, 0.15f, Tween.Type.QUAD_IN_OUT) {
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
}));
CurrentSettings.Add(PauseMenuUi.Add(new UiCheckbox("uisfx", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 2)) {
public override void OnClick() {
Settings.Uisfx = !Settings.Uisfx;
base.OnClick();
}
}.SetOn(Settings.Uisfx)));
CurrentSettings.Add(PauseMenuUi.Add(new UiSlider("sfx", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 3)) {
public override void OnUpdate() {
Settings.Sfx = this.Val;
}
}.SetValue(Settings.Sfx)));
UiEntity Button = (UiEntity) PauseMenuUi.Add(new UiSlider("music", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 4)) {
public override void OnUpdate() {
Settings.Music = this.Val;
Audio.Update();
}
}.SetValue(Settings.Music));
CurrentSettings.Add(Button);
PauseMenuUi.Select(Button);
}
public void AddGame() {
Clear();
float S = 18;
float St = 60 - 2.5f;
CurrentSettings.Add(PauseMenuUi.Add(new UiButton("back", (int) (Display.UI_WIDTH * 2.5f), (int) (St)) {
public override void Render() {
base.Render();
if (SettingsX == Display.UI_WIDTH * 2 && Input.Instance.WasPressed("pause")) {
Input.Instance.PutState("pause", Input.State.UP);
this.OnClick();
}
}
public override void OnClick() {
Audio.PlaySfx("menu/exit");
SelectGraphics();
Tween.To(new Tween.Task(Display.UI_WIDTH * 1f, 0.15f, Tween.Type.QUAD_IN_OUT) {
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
}));
CurrentSettings.Add(PauseMenuUi.Add(new UiButton("view_credits", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 2)) {
public override void OnClick() {
base.OnClick();
}
}));
CurrentSettings.Add(PauseMenuUi.Add(new UiButton("reset_progress", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 3)) {
public override void OnClick() {
base.OnClick();
Tween.To(new Tween.Task(0, 0.1f) {
public override float GetValue() {
return Dungeon.Dark;
}
public override void SetValue(float Value) {
Dungeon.Dark = Value;
}
public override void OnEnd() {
SaveManager.DeleteAll();
Dungeon.Depth = -2;
GameSave.RunId = 0;
Dungeon.LoadType = Entrance.LoadType.GO_DOWN;
Player.Instance = null;
Player.Ladder = null;
BurningKnight.Instance = null;
Dungeon.Game.SetState(new MainMenuState());
}
public override bool RunWhenPaused() {
return true;
}
});
}
}));
CurrentSettings.Add(PauseMenuUi.Add(new UiCheckbox("speedrun_timer", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 5)) {
public override void OnClick() {
Settings.Speedrun_timer = !Settings.Speedrun_timer;
Dungeon.TweenTimer(Settings.Speedrun_timer);
base.OnClick();
}
}.SetOn(Settings.Speedrun_timer)));
CurrentSettings.Add(PauseMenuUi.Add(new UiCheckbox("blood_gore", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 6)) {
public override void OnClick() {
Settings.Blood = !Settings.Blood;
Settings.Gore = !Settings.Gore;
base.OnClick();
}
}.SetOn(Settings.Gore)));
CurrentSettings.Add(PauseMenuUi.Add(new UiCheckbox("vegan_mode", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 7)) {
private int Clicks;
public override void OnClick() {
Clicks++;
if (Clicks == 15) {
foreach (string Id in NpcSaveRoom.SaveOrder) {
GlobalSave.Put("npc_" + Id + "_saved", true);
}
GlobalSave.Put("all_npcs_saved", true);
Audio.PlaySfx("curse_lamp", 1f, 1f);
}
Settings.Vegan = !Settings.Vegan;
base.OnClick();
}
}.SetOn(Settings.Vegan)));
UiEntity Button = (UiEntity) PauseMenuUi.Add(new UiCheckbox("show_fps", (int) (Display.UI_WIDTH * 2.5f), (int) (St + S * 8)) {
public override void Update(float Dt) {
base.Update(Dt);
SetOn(Dungeon.FpsY != 0);
}
public override void OnClick() {
Tween.To(new Tween.Task(Dungeon.FpsY == 0 ? 18 : 0, 0.3f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return Dungeon.FpsY;
}
public override void SetValue(float Value) {
Dungeon.FpsY = Value;
}
public override bool RunWhenPaused() {
return true;
}
});
base.OnClick();
}
}.SetOn(Dungeon.FpsY != 0));
CurrentSettings.Add(Button);
PauseMenuUi.Select(Button);
}
}
}