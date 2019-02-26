using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.ui {
	public class UiMap : UiEntity {
		public static UiMap Instance;
		public static bool Large;
		private static TextureRegion Frame = Graphics.GetTexture("ui-minimap");
		private static TextureRegion TopLeft = Graphics.GetTexture("ui-map_top_left");
		private static TextureRegion Top = Graphics.GetTexture("ui-map_top");
		private static TextureRegion TopRight = Graphics.GetTexture("ui-map_top_right");
		private static TextureRegion Left = Graphics.GetTexture("ui-map_left");
		private static TextureRegion Right = Graphics.GetTexture("ui-map_right");
		private static TextureRegion BottomLeft = Graphics.GetTexture("ui-map_bottom_left");
		private static TextureRegion Bottom = Graphics.GetTexture("ui-map_bottom");
		private static TextureRegion BottomRight = Graphics.GetTexture("ui-map_bottom_right");
		private static Color Color = Color.ValueOf("#424c6e");
		public static TextureRegion Entrance = Graphics.GetTexture("ui-entrance");
		public static TextureRegion Exit = Graphics.GetTexture("ui-exit");
		public static TextureRegion Shop = Graphics.GetTexture("ui-shop");
		private Color Bg = Color.ValueOf("#2a2f4e");
		private Color Border = Color.ValueOf("#0e071b");
		private bool Did;
		private bool HadOpen;
		private UiButton Hide;
		private Tween.Task Last;
		private float Ly;
		private UiButton Minus;
		private UiButton MinusLarge;
		private float My;
		private UiButton Plus;
		private UiButton PlusLarge;
		private UiButton Show;
		private float Speed = 0.5f;
		private bool ToRemove;
		private float Xc;
		private float Yc;
		private float Zoom = GlobalSave.GetFloat("minimap_zoom") == 0 ? 0.5f : GlobalSave.GetFloat("minimap_zoom");

		protected void _Init() {
			{
				Depth = 16;
				IsSelectable = false;
			}
		}

		public bool IsOpen() {
			return Large || My == 0;
		}

		public void Hide() {
			Tween.Remove(Last);
			Last = Tween.To(new Tween.Task(96, 0.2f) {

		public override float GetValue() {
			return My;
		}

		public override void SetValue(float Value) {
			My = Value;
		}

		public override void OnEnd() {
			base.OnEnd();
			Did = false;
		}

		public override bool RunWhenPaused() {
			return true;
		}
	});

	Did = true;
	GlobalSave.Put("hide_minimap", true);
}

public void Remove() {
if (!this.Did) {
ToRemove = true;
if (this.My == 0) {
	this.Hide();
	GlobalSave.Put("hide_minimap", false);
}
if (Large) {
this.HideHuge();
}
}
}
public void Show() {
Tween.Remove(Last);
Last = Tween.To(new Tween.Task(0, 0.4f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return My;
}
public override void SetValue(float Value) {
My = Value;
}
public override void OnEnd() {
base.OnEnd();
Did = false;
}
public override bool RunWhenPaused() {
return true;
}
});
Did = true;
GlobalSave.Put("hide_minimap", false);
}
public override void Destroy() {
base.Destroy();
Instance = null;
}
public override void Init() {
base.Init();
Large = false;
Instance = this;
SetSize();
My = 0;
UiMap Self = this;
this.PlusLarge = new UiImageButton("ui-plus", (int) (X + W - 30), (int) Y) {
public override void OnClick() {
base.OnClick();
Plus();
}
public override void Update(float Dt) {
if (Large && Dungeon.Depth >= 0) {
base.Update(Dt);
}
}
public void Render() {
if (Large && Dungeon.Depth >= 0 && !Ui.HideUi) {
Y = Self.Y + Ly - 2;
base.Render();
}
}
};
Dungeon.Ui.Add(PlusLarge);
this.MinusLarge = new UiImageButton("ui-minus", (int) (X + W - 20), (int) Y) {
public override void OnClick() {
base.OnClick();
Minus();
}
public override void Update(float Dt) {
if (Large && Dungeon.Depth >= 0) {
base.Update(Dt);
}
}
public void Render() {
if (Large && Dungeon.Depth >= 0 && !Ui.HideUi) {
base.Render();
Y = Self.Y + Ly - 2;
}
}
};
Dungeon.Ui.Add(MinusLarge);
}
protected void Plus() {
Zoom = Math.Min(2f, Zoom + Speed);
GlobalSave.Put("minimap_zoom", Zoom);
}
protected void Minus() {
Zoom = Math.Max(0.25f, Zoom - Speed);
GlobalSave.Put("minimap_zoom", Zoom);
}
public void SetSize() {
this.W = Large ? Display.UI_WIDTH - 20 : 64;
this.H = Large ? Display.UI_HEIGHT - 20 : Math.Min(this.W, Display.UI_HEIGHT);
this.X = Math.Round(Display.UI_WIDTH - this.W - (Large ? 10 : 4));
this.Y = Math.Round(Display.UI_HEIGHT - this.H - (Large ? 10 : 4));
}
public override void Update(float Dt) {
if (Dungeon.Depth < 0) {
return;
}
base.Update(Dt);
if (Dialog.Active == null) {
if (Input.Instance.WasPressed("zoom_out")) {
Minus();
}
if (Input.Instance.WasPressed("zoom_in")) {
Plus();
}
if (!Did && Input.Instance.WasPressed("toggle_minimap") && !Large) {
if (My == 0) {
Hide();
} else {
Show();
}
}
if (Large) {
float S = 30f * (1 / Zoom);
if (Input.Instance.IsDown("left")) {
Xc += S * Dt;
}
if (Input.Instance.IsDown("right")) {
Xc -= S * Dt;
}
if (Input.Instance.IsDown("up")) {
Yc -= S * Dt;
}
if (Input.Instance.IsDown("down")) {
Yc += S * Dt;
}
Vector2 Move = Input.Instance.GetAxis("move");
if (Move.Len2() > 0.2) {
Xc += Move.X * Dt * S;
Yc += Move.Y * Dt * S;
}
}
if (Input.Instance.WasPressed("map") && !Dungeon.Game.GetState().IsPaused() && !Did) {
if (!Large) {
if (!Player.Instance.IsDead()) {
OpenHuge();
}
} else {
HideHuge();
}
}
}
}
public void OpenHuge() {
Xc = 0;
Yc = 0;
if (!Large && My != 96) {
HadOpen = true;
Did = true;
Tween.Remove(Last);
Last = Tween.To(new Tween.Task(96, 0.1f) {
public override float GetValue() {
return My;
}
public override void SetValue(float Value) {
My = Value;
}
public override bool RunWhenPaused() {
return true;
}
public override void OnEnd() {
base.OnEnd();
DoLarge();
}
});
} else {
HadOpen = false;
DoLarge();
}
}
private void DoLarge() {
Large = true;
SetSize();
Ly = -Display.UI_HEIGHT;
Tween.To(new Tween.Task(0, 0.3f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return Ly;
}
public override void SetValue(float Value) {
Ly = Value;
}
public override void OnEnd() {
Did = false;
}
public override bool RunWhenPaused() {
return true;
}
});
}
public void HideHuge() {
Tween.To(new Tween.Task(-Display.UI_HEIGHT, 0.1f) {
public override float GetValue() {
return Ly;
}
public override void SetValue(float Value) {
Ly = Value;
}
public override void OnEnd() {
Large = false;
SetSize();
Did = false;
if (!ToRemove && HadOpen) {
Tween.Remove(Last);
Last = Tween.To(new Tween.Task(0, 0.2f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return My;
}
public override void SetValue(float Value) {
My = Value;
}
public override void OnEnd() {
Did = false;
}
public override bool RunWhenPaused() {
return true;
}
});
Did = true;
HadOpen = false;
}
ToRemove = false;
}
public override bool RunWhenPaused() {
return true;
}
});
Did = true;
}
public override void Render() {
if (!Large) {
return;
}
if ((My == 96 && !Large) || Dungeon.Depth < 0 || Ui.HideUi) {
return;
}
Graphics.StartAlphaShape();
Graphics.Shape.SetProjectionMatrix(Camera.Ui.Combined);
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
if (Large) {
Graphics.Shape.SetColor(Color.R, Color.G, Color.B, 0.8f);
Graphics.Shape.Rect(this.X + 1, this.Y + 1 + Ly, this.W - 1, this.H - 1);
} else {
Graphics.Shape.SetColor(Bg);
Graphics.Shape.Rect(this.X + 1, this.Y + this.My + 1, this.W - 2, this.H - 2);
}
Graphics.EndAlphaShape();
Graphics.Batch.End();
Graphics.Shadows.End(Camera.Viewport.GetScreenX(), Camera.Viewport.GetScreenY(), Camera.Viewport.GetScreenWidth(), Camera.Viewport.GetScreenHeight());
Graphics.Map.Begin();
Gdx.Gl.GlClearColor(0, 0, 0, 0);
Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT | (Gdx.Graphics.GetBufferFormat().CoverageSampling ? GL20.GL_COVERAGE_BUFFER_BIT_NV : 0));
Graphics.Shape.SetProjectionMatrix(Camera.Ui.Combined);
Gdx.Gl.GlEnable(GL20.GL_BLEND);
Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
float S = Math.Round((Large ? 6 : 4) * Zoom);
float Px = Player.Instance.X + Player.Instance.W / 2f - (Large ? Xc * S : 0);
float Py = Player.Instance.Y + Player.Instance.H / 2f - (Large ? Yc * S : 0);
float Mx = -Px / (16f / S) + this.W / 2 + (Large ? Xc : 0);
float My = -Py / (16f / S) + this.H / 2 + (Large ? Yc : 0);
float O = 1f;
int Xx;
int Yy;
Graphics.Shape.SetColor(Border);
float Cx = Px - (W * 2) / (Zoom / 1.5f);
float Cy = Py - (H * 2) / (Zoom / 1.5f);
int Sx = (int) (Math.Floor(Cx / 16) - 1);
int Sy = (int) (Math.Floor(Cy / 16) - 1);
int Fx = (int) (Math.Ceil((Cx + W * 4 / (Zoom / 1.5f)) / 16) + 1);
int Fy = (int) (Math.Ceil((Cy + H * 4 / (Zoom / 1.5f)) / 16) + 1);
Xx = Math.Max(0, Sx);
for (int X = Math.Max(0, Sx); X < Math.Min(Fx, Level.GetWidth()); X++) {
Yy = Math.Max(0, Sy);
for (int Y = Math.Max(0, Sy); Y < Math.Min(Fy, Level.GetHeight()); Y++) {
byte T = Dungeon.Level.Get(X, Y);
if (T >= 0 && T != Terrain.WALL && T != Terrain.CRACK) {
if (Dungeon.Level.Explored(X, Y)) {
Graphics.Shape.Rect(Xx * S - O + Mx, Yy * S - O + My, S + O * 2, S + O * 2);
}
}
Yy++;
}
Xx++;
}
Xx = Math.Max(0, Sx);
for (int X = Math.Max(0, Sx); X < Math.Min(Fx, Level.GetWidth()); X++) {
Yy = Math.Max(0, Sy);
for (int Y = Math.Max(0, Sy); Y < Math.Min(Fy, Level.GetHeight()); Y++) {
if (Dungeon.Level.Explored(X, Y)) {
int I = Level.ToIndex(X, Y);
byte L = Dungeon.Level.LiquidData[I];
byte T = Dungeon.Level.Data[I];
if (T >= 0 && T != Terrain.WALL && T != Terrain.CRACK) {
if (L == 0 || L == Terrain.COBWEB || L == Terrain.EMBER) {
L = T;
}
Color Color = Terrain.GetColor(L);
Graphics.Shape.SetColor(Color == null ? Color.WHITE : Color);
Graphics.Shape.Rect(Xx * S + Mx, Yy * S + My, S, S);
}
}
Yy++;
}
Xx++;
}
float Plx = Player.Instance.X / 16f;
float Ply = Player.Instance.Y / 16f;
Graphics.Shape.SetColor(0, 1, 0, 1);
Graphics.Shape.Rect(Plx * S + S / 4f + Mx, Ply * S + S / 4f + My, S / 2f, S / 2f);
Graphics.Shape.SetColor(1, 1, 1, 1);
Graphics.Shape.End();
Gdx.Gl.GlDisable(GL20.GL_BLEND);
Graphics.Batch.Begin();
foreach (Room Room in Dungeon.Level.GetRooms()) {
if (Dungeon.Level.Explored[Level.ToIndex(Room.Left + 1, Room.Top + 1)]) {
if (Room is ShopRoom) {
Graphics.Render(Shop, (Room.Left + ((float) Room.GetWidth()) / 2f) * S + Mx, (Room.Top + ((float) Room.GetHeight()) / 2f) * S + My, 0, Shop.GetRegionWidth() / 2, Shop.GetRegionHeight() / 2, false, false);
} else if (Room is EntranceRoom && !(Room is BossRoom) || Room is BossEntranceRoom) {
TextureRegion Reg = Room is BossEntranceRoom ? Exit : (((EntranceRoom) Room).Exit ? Exit : Entrance);
Graphics.Render(Reg, (Room.Left + ((float) Room.GetWidth()) / 2f) * S + Mx, (Room.Top + ((float) Room.GetHeight()) / 2f) * S + My, 0, Reg.GetRegionWidth() / 2, Reg.GetRegionHeight() / 2, false, false);
}
}
}
Graphics.Batch.End();
Graphics.Map.End(Camera.Viewport.GetScreenX(), Camera.Viewport.GetScreenY(), Camera.Viewport.GetScreenWidth(), Camera.Viewport.GetScreenHeight());
Graphics.Shadows.Begin();
Texture Texture = Graphics.Map.GetColorBufferTexture();
Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
Graphics.Batch.Begin();
if (!Large) {
Graphics.Batch.Draw(Texture, this.X + 1, this.Y + 1 + this.My, this.W - 2, this.H - 2, 0, 0, (int) this.W - 2, (int) this.H - 2, false, true);
Graphics.Render(Frame, X, Y + this.My);
} else {
Graphics.Batch.Draw(Texture, this.X + 1, this.Y + 1 + Ly, this.W - 2, this.H - 2, 0, 0, (int) this.W - 2, (int) this.H - 2, false, true);
RenderLarge();
}
}
private void RenderLarge() {
float Sx = (this.W - 10);
float Sy = (this.H - 10);
Graphics.Render(BottomLeft, X, Y + Ly);
Graphics.Render(Bottom, X + BottomLeft.GetRegionWidth(), Y + Ly, 0, 0, 0, false, false, Sx, 1);
Graphics.Render(BottomRight, X + this.W - BottomRight.GetRegionWidth(), Y + Ly);
Graphics.Render(Left, X, Y + Ly + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
Graphics.Render(Right, X + this.W - Right.GetRegionWidth(), Y + Ly + BottomLeft.GetRegionHeight(), 0, 0, 0, false, false, 1, Sy);
Graphics.Render(TopLeft, X, Y + Ly + H - TopLeft.GetRegionHeight());
Graphics.Render(Top, X + TopLeft.GetRegionWidth(), Y + Ly + H - TopLeft.GetRegionHeight(), 0, 0, 0, false, false, Sx, 1);
Graphics.Render(TopRight, X + this.W - TopRight.GetRegionWidth(), Y + Ly + H - TopLeft.GetRegionHeight());
}
public UiMap() {
_Init();
}
}
}