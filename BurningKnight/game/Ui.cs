using BurningKnight.entity.creature.mob.boss;
using BurningKnight.entity.creature.player.fx;
using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.game {
	public class Ui {
		public static Ui Ui;
		public static bool HideCursor;
		public static bool HideUi;
		public static TextureRegion Save;
		public static float Upscale = 1;
		public static TextureRegion[] Regions;
		private static string KillsLocale = Locale.Get("kills");
		private static TextureRegion Coin;
		public static float Y;
		private static string WonLocale = Locale.Get("you_won");
		private static string KillLocale = Locale.Get("didnt_kill_bk");
		private static string Kill2Locale = Locale.Get("killed_yet_dead");
		private static string HintStr = Locale.Get("upgrade_hint");
		public static bool Move;
		public static bool UpgradeMouse;
		public static bool DrawHint = true;
		private static float Alf;
		public static float SaveAlpha;
		private float Al;
		public bool Dead;
		private string Depth;
		public Dictionary<Class, Healthbar> Healthbars = new Dictionary<>();
		private string Kills;
		private float KillX = -Display.UI_WIDTH;
		private float MainY = -128;
		private ItemPickupFx PickupFx;
		private float Scale = 1f;
		private float Size;
		private string Time;
		private float TimeW;
		private TextureRegion Upgrade;
		private float Val;
		private bool Won;

		public Ui() {
			Ui = this;
			Upgrade = Graphics.GetTexture("ui-upgrade_cursor");
			Save = Graphics.GetTexture("ui-save");
			Regions =  {
				Graphics.GetTexture("ui-cursor-standart"), Graphics.GetTexture("ui-cursor-small"), Graphics.GetTexture("ui-cursor-rect"), Graphics.GetTexture("ui-cursor-corner"), Graphics.GetTexture("ui-cursor-sniper"), Graphics.GetTexture(
					"ui-cursor-round-sniper"), Graphics.GetTexture("ui-cursor-cross"), Graphics.GetTexture("ui-cursor-nt"), null
			}
			;
		}

		public void Update(float Dt) {
			SaveAlpha += ((SaveManager.Saving > 0 ? 1 : 0) - SaveAlpha) * Dt * 4;
			SaveManager.Saving -= Dt;

			if (!Dead)
				foreach (Boss Boss in Boss.All)
					if (Boss.Talked && !Boss.GetState().Equals("unactive") && !Healthbars.ContainsKey(Boss.GetClass())) {
						var Healthbar = new Healthbar();
						Healthbar.Boss = Boss;
						Healthbar.TargetValue = Healthbars.Size() * 19 + 16;
						Healthbars.Put(Boss.GetClass(), Healthbar);
					}

			Healthbar[] Bars = Healthbars.Values().ToArray({
			});

			for (var I = Bars.Length - 1; I >= 0; I--) {
				Bars[I].Update(Dt);

				if (Bars[I].Done || Dead) {
					Healthbars.Remove(Bars[I].Boss.GetClass());
					var J = 0;

					foreach (Healthbar Bar in Healthbars.Values()) {
						Bar.TargetValue = J * 19 + 16;
						Bar.Tweened = true;
						Tween.To(new Tween.Task(Display.UI_HEIGHT - Bar.TargetValue, 0.5f) {

		public override float GetValue() {
			return Bar.Y;
		}

		public override void SetValue(float Value) {
			Bar.Y = Value;
		}
	});

	J++;
}

}
}
if (Input.Instance.WasPressed("use")) {
Tween.To(new Tween.Task(1.25f, 0.03f) {
public override float GetValue() {
return Scale;
}
public override void SetValue(float Value) {
Scale = Value;
}
public override void OnEnd() {
base.OnEnd();
Tween.To(new Tween.Task(1f, 0.1f, Tween.Type.BACK_IN_OUT) {
public override float GetValue() {
return Scale;
}
public override void SetValue(float Value) {
Scale = Value;
}
public override void OnEnd() {
base.OnEnd();
}
});
}
});
}
}
public void Reset() {
MainY = -128;
KillX = -Display.UI_WIDTH;
Size = 0;
}
public void OnWin() {
Won = true;
SaveManager.Delete();
Depth = Dungeon.Level == null ? "Unknown" : Dungeon.Level.FormatDepth();
Kills = GameSave.KillCount + " " + KillsLocale;
Time = string.Format("%02d:%02d:%02d.%02d", (int) Math.Floor(GameSave.Time / 3600), (int) Math.Floor(GameSave.Time / 60), (int) Math.Floor(GameSave.Time % 60), ((int) Math.Floor(GameSave.Time % 1 * 100)));
Graphics.Layout.SetText(Graphics.Small, Time);
TimeW = Graphics.Layout.Width;
Val = 1;
Tween.To(new Tween.Task(0, 1f) {
public override void OnEnd() {
{
Tween.To(new Tween.Task(1f, 0.3f) {
public override float GetValue() {
return Dungeon.Grayscale;
}
public override void SetValue(float Value) {
Dungeon.Grayscale = Value;
}
}).Delay(0.15f);
Tween.To(new Tween.Task(1, 0.05f) {
public override float GetValue() {
return Al;
}
public override void SetValue(float Value) {
Al = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(0, 0.4f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return MainY;
}
public override void SetValue(float Value) {
MainY = Value;
}
});
Tween.To(new Tween.Task(52, 0.2f) {
public override float GetValue() {
return Size;
}
public override void SetValue(float Value) {
Size = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(0, 0.4f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return KillX;
}
public override void SetValue(float Value) {
KillX = Value;
}
});
}
});
Tween.To(new Tween.Task(0, 0.1f) {
public override float GetValue() {
return Al;
}
public override void SetValue(float Value) {
Al = Value;
}
});
}
});
Audio.Play("Nostalgia");
Audio.Reset();
UiButton Button = (UiButton) Dungeon.Ui.Add(new UiButton("play_again", Display.UI_WIDTH / 2 + Display.UI_WIDTH, 107 + 24) {
public override void OnClick() {
base.OnClick();
Rst();
InGameState.StartTween = true;
InGameState.NewGame = true;
}
});
UiButton FinalButton3 = Button;
Tween.To(new Tween.Task(Display.UI_WIDTH / 2, 0.5f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return FinalButton3.X;
}
public override void SetValue(float Value) {
FinalButton3.X = Value;
}
}).Delay(0.3f);
Dungeon.Ui.Select(Button);
if (Dungeon.Depth != -3) {
Button = (UiButton) Dungeon.Ui.Add(new UiButton("back_to_castle", Display.UI_WIDTH / 2 - Display.UI_WIDTH, 107) {
public override void OnClick() {
base.OnClick();
Rst();
Dungeon.BackToCastle(true, -2);
Camera.Shake(3);
}
});
UiButton FinalButton = Button;
Tween.To(new Tween.Task(Display.UI_WIDTH / 2, 0.5f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return FinalButton.X;
}
public override void SetValue(float Value) {
FinalButton.X = Value;
}
}).Delay(0.3f);
Button = (UiButton) Dungeon.Ui.Add(new UiButton("menu", Display.UI_WIDTH / 2 + Display.UI_WIDTH, 83) {
public override void OnClick() {
base.OnClick();
Rst();
State.Transition(new Runnable() {
public override void Run() {
Dungeon.Game.SetState(new MainMenuState());
}
});
Camera.Shake(3);
}
});
UiButton FinalButton1 = Button;
Tween.To(new Tween.Task(Display.UI_WIDTH / 2, 0.5f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return FinalButton1.X;
}
public override void SetValue(float Value) {
FinalButton1.X = Value;
}
}).Delay(0.3f);
}
}
}
});
}
public void OnDeath() {
Won = false;
SaveManager.Delete();
Depth = Dungeon.Level == null ? "Unknown" : Dungeon.Level.FormatDepth();
Kills = GameSave.KillCount + " " + KillsLocale;
Time = string.Format("%02d:%02d:%02d.%02d", (int) Math.Floor(GameSave.Time / 3600), (int) Math.Floor(GameSave.Time / 60), (int) Math.Floor(GameSave.Time % 60), ((int) Math.Floor(GameSave.Time % 1 * 100)));
Graphics.Layout.SetText(Graphics.Small, Time);
TimeW = Graphics.Layout.Width;
Val = 1;
Tween.To(new Tween.Task(0, 1f) {
public override void OnEnd() {
{
Tween.To(new Tween.Task(1f, 0.3f) {
public override float GetValue() {
return Dungeon.Grayscale;
}
public override void SetValue(float Value) {
Dungeon.Grayscale = Value;
}
}).Delay(0.15f);
Tween.To(new Tween.Task(1, 0.05f) {
public override float GetValue() {
return Al;
}
public override void SetValue(float Value) {
Al = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(0, 0.4f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return MainY;
}
public override void SetValue(float Value) {
MainY = Value;
}
});
Tween.To(new Tween.Task(52, 0.2f) {
public override float GetValue() {
return Size;
}
public override void SetValue(float Value) {
Size = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(0, 0.4f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return KillX;
}
public override void SetValue(float Value) {
KillX = Value;
}
});
}
});
Tween.To(new Tween.Task(0, 0.1f) {
public override float GetValue() {
return Al;
}
public override void SetValue(float Value) {
Al = Value;
}
});
}
});
Audio.Play("Nostalgia");
Audio.Reset();
UiButton Button = (UiButton) Dungeon.Ui.Add(new UiButton("restart", Display.UI_WIDTH / 2 + Display.UI_WIDTH, 107 + 24) {
public override void OnClick() {
base.OnClick();
Rst();
InGameState.StartTween = true;
InGameState.NewGame = true;
}
});
Dungeon.Ui.Select(Button);
UiButton FinalButton3 = Button;
Tween.To(new Tween.Task(Display.UI_WIDTH / 2, 0.5f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return FinalButton3.X;
}
public override void SetValue(float Value) {
FinalButton3.X = Value;
}
}).Delay(0.3f);
if (Dungeon.Depth != -3) {
Button = (UiButton) Dungeon.Ui.Add(new UiButton("back_to_castle", Display.UI_WIDTH / 2 - Display.UI_WIDTH, 107) {
public override void OnClick() {
base.OnClick();
Rst();
Dungeon.BackToCastle(true, -2);
Camera.Shake(3);
}
});
UiButton FinalButton = Button;
Tween.To(new Tween.Task(Display.UI_WIDTH / 2, 0.5f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return FinalButton.X;
}
public override void SetValue(float Value) {
FinalButton.X = Value;
}
}).Delay(0.3f);
Button = (UiButton) Dungeon.Ui.Add(new UiButton("menu", Display.UI_WIDTH / 2 + Display.UI_WIDTH, 83) {
public override void OnClick() {
base.OnClick();
Rst();
State.Transition(new Runnable() {
public override void Run() {
Dungeon.Game.SetState(new MainMenuState());
}
});
Camera.Shake(3);
}
});
UiButton FinalButton1 = Button;
Tween.To(new Tween.Task(Display.UI_WIDTH / 2, 0.5f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return FinalButton1.X;
}
public override void SetValue(float Value) {
FinalButton1.X = Value;
}
}).Delay(0.3f);
}
}
}
});
}
private void Rst() {
MainY = -128;
KillX = -Display.UI_WIDTH;
Tween.To(new Tween.Task(0, 0.3f) {
public override float GetValue() {
return Dungeon.Grayscale;
}
public override void SetValue(float Value) {
Dungeon.Grayscale = Value;
}
});
Tween.To(new Tween.Task(0, 0.3f) {
public override float GetValue() {
return Size;
}
public override void SetValue(float Value) {
Size = Value;
}
});
}
public static void RenderSaveIcon(float Upscale) {
if (Ui.HideUi) {
return;
}
if (SaveAlpha > 0.05f) {
Upscale = Math.Max(1, Upscale * 0.7f);
Graphics.Batch.SetColor(1, 1, 1, (float) Math.Max(0, SaveAlpha + Math.Sin(Dungeon.Time * 8) * 0.1f - 0.1f));
Graphics.Render(Save, Display.UI_WIDTH - 4 - Save.GetRegionWidth() * Upscale, 4, 0, 0, 0, false, false, Upscale, Upscale);
Graphics.Batch.SetColor(1, 1, 1, 1);
}
}
public void Render() {
if (HideUi) {
return;
}
if (Coin == null) {
Coin = Graphics.GetTexture("ui-coin");
}
if (Player.Instance != null && (Player.Instance.PickupFx != null || PickupFx != null)) {
if (Player.Instance.PickupFx != null) {
PickupFx = Player.Instance.PickupFx;
}
Item Item = PickupFx.Item.GetItem();
Item.SetOwner(Player.Instance);
string Info = Item.BuildInfo().ToString();
Graphics.Layout.SetText(Graphics.Small, Info);
Graphics.Print(Info, Graphics.Small, Display.UI_WIDTH - Graphics.Layout.Width - 4, (Graphics.Layout.Height) * PickupFx.Item.Al - (1 - PickupFx.Item.Al) * Graphics.Layout.Height);
if (PickupFx.Item.Al <= 0.05f) {
PickupFx = null;
} else if (PickupFx.Done) {
PickupFx.Item.Al = (PickupFx.Item.Al - Gdx.Graphics.GetDeltaTime() * 3);
}
}
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
foreach (Healthbar Healthbar in Healthbars.Values()) {
Healthbar.Render();
}
if (Dungeon.Game.GetState() is InGameState) {
if (Dungeon.Depth == -2 || Y > 0) {
float Mx = Dungeon.FpsY * 0.5f + Dungeon.TimerY * 3;
Graphics.Render(Coin, 4 + Mx, Display.UI_HEIGHT - (Dungeon.Depth <= -1 ? 16 : Y - 4) + 1);
Graphics.Print(GlobalSave.GetInt("num_coins") + "", Graphics.Small, 17 + Mx, Display.UI_HEIGHT - (Dungeon.Depth <= -1 ? 14 : Y - 6));
}
if (this.Al > 0.05f && !Ui.HideUi) {
Graphics.StartAlphaShape();
Graphics.Shape.SetColor(this.Val, this.Val, this.Val, this.Al);
Graphics.Shape.Rect(0, 0, Display.UI_WIDTH, Display.UI_HEIGHT);
Graphics.EndAlphaShape();
}
float Y = Display.UI_HEIGHT - 52 - 32;
if (this.Size > 0) {
Graphics.StartShape();
Graphics.Shape.SetColor(0, 0, 0, 1);
Graphics.Shape.Rect(0, 0, Display.UI_WIDTH, Size);
Graphics.Shape.Rect(0, Display.UI_HEIGHT - Size, Display.UI_WIDTH, Size);
Graphics.EndShape();
if (this.KillX != -128) {
float Yy = Y - 32;
Graphics.Small.Draw(Graphics.Batch, this.Depth, this.KillX + 32, Yy - 16);
Graphics.Small.Draw(Graphics.Batch, this.Kills, this.KillX + 32, Yy);
Graphics.Small.Draw(Graphics.Batch, this.Time, Display.UI_WIDTH - 32 - this.KillX - this.TimeW, Yy);
}
}
if (this.MainY != -128) {
Graphics.Print(Won ? WonLocale : KillLocale, Graphics.Medium, Y - 16 + this.MainY);
}
}
}
public void RenderCursor() {
if (HideCursor) {
return;
}
if (UpgradeMouse) {
Graphics.Batch.SetColor(1, 1, 1, 1);
Graphics.Render(this.Upgrade, Input.Instance.UiMouse.X, Input.Instance.UiMouse.Y, 0, this.Upgrade.GetRegionWidth() / 2, this.Upgrade.GetRegionHeight(), false, false);
Alf += ((Move ? 1 : 0) - Alf) * Gdx.Graphics.GetDeltaTime() * 8;
if (Alf > 0.05f && DrawHint) {
Graphics.Medium.SetColor(0.3f, 1f, 0.3f, Alf);
Graphics.Print(HintStr, Graphics.Medium, Input.Instance.UiMouse.X + 8, Input.Instance.UiMouse.Y - 20);
Graphics.Medium.SetColor(1, 1, 1, 1);
}
} else {
TextureRegion Region = Regions[Settings.CursorId];
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
if (Region == null) {
return;
}
float S = Settings.RotateCursor ? (float) (1.2f + Math.Cos(Dungeon.Time / 1.5f) / 5f) * this.Scale : this.Scale;
float A = Settings.RotateCursor ? Dungeon.Time * 60 : 0;
Graphics.Render(Region, Input.Instance.UiMouse.X, Input.Instance.UiMouse.Y, A, ((float) Region.GetRegionWidth()) / 2, ((float) Region.GetRegionHeight()) / 2, false, false, S, S);
if (Dungeon.Game.GetState() is ItemSelectState && StartingItem.Hovered != null) {
Graphics.Print(StartingItem.Hovered.Name, Graphics.Medium, Input.Instance.UiMouse.X + 10, Input.Instance.UiMouse.Y - 13);
}
}
}
}
}