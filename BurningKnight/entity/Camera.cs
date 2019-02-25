using BurningKnight.entity.level.rooms;
using BurningKnight.util;

namespace BurningKnight.entity {
	public class Camera : Entity {
		public static bool Did = false;
		public static Camera Instance;
		public static OrthographicCamera Ui;
		public static OrthographicCamera Nil;
		public static OrthographicCamera Game;
		public static OrthographicCamera ViewportCamera;
		public static Viewport Viewport;
		public static Entity Target;
		private static float Shake;
		private static float PushA;
		private static float PushAm;
		private static float St;
		public static bool NoMove;
		private static Tween.Task Last;
		private static Vector2 CamPosition;
		private static Room LastRoom;
		private static Vector2 MousePosition = new Vector2();
		private static float T;
		private static Vector2 Offset = new Vector2();
		private static Vector2 Velocity = new Vector2();
		private static bool IgnoreMouse;
		public static float Ma;
		private static float Speed;

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		public void ResetShake() {
			Shake = 0;
			PushA = 0;
			PushAm = 0;
			St = 0;
		}

		public static void Push(float A, float Am) {
			PushA = A;
			PushAm = 0;

			if (Last != null) Tween.Remove(Last);

			Last = Tween.To(new Tween.Task(Am * Settings.Screenshake, 0.05f) {

		public override float GetValue() {
			return PushAm;
		}

		public override void SetValue(float Value) {
			PushAm = Value;
		}
	});
}

public Camera() {
internal _Init();
	if (Instance != null) {
	Instance.Done = true;
	Instance = this;

	return;
}

Instance = this;
internal int W = Display.GAME_WIDTH;
internal int H = Display.GAME_HEIGHT;
Ui =
internal new OrthographicCamera(Display.UI_WIDTH, Display.UI_HEIGHT);
Ui.Position.Set(Display.UI_WIDTH / 2, Display.UI_HEIGHT / 2, 0);
Ui.Update();
Nil =
internal new OrthographicCamera(W, H);
Nil.Position.Set(Display.GAME_WIDTH / 2, Display.GAME_HEIGHT / 2, 0);
Nil.Update();
AlwaysActive = true;
Game =
internal new OrthographicCamera(W, H);
Game.Position.Set(Game.ViewportWidth / 2, Game.ViewportHeight / 2, 0);
Game.Update();
CamPosition =
internal new Vector2(Game.Position.X, Game.Position.Y);
ViewportCamera =
internal new OrthographicCamera(Display.GAME_WIDTH, Display.GAME_HEIGHT);
ViewportCamera.Update();
Viewport =
internal new ScreenViewport(ViewportCamera);
Viewport.Update(Gdx.Graphics.GetWidth(), Gdx.Graphics.GetHeight());
}

public static void Resize(int Width, int Height) {
Viewport.Update(Width, Height);
}

public override void Init() {
base.Init();
Depth = 80;
MousePosition.Set(Input.Instance.UiMouse.X, Input.Instance.UiMouse.Y);
}
public override void Update(float Dt) {
if (Last != null && Last.Done) {
Last = null;
}
if (Version.Debug) {
if (Gdx.Input.IsKeyJustPressed(Com.Badlogic.Gdx.Input.Keys.NUMPAD_0)) {
if (Target == null) {
Follow(Player.Instance, false);
} else {
Follow(null);
}
}
if (Gdx.Input.IsKeyJustPressed(Com.Badlogic.Gdx.Input.Keys.NUMPAD_7)) {
IgnoreMouse = !IgnoreMouse;
}
if (Gdx.Input.IsKeyJustPressed(Com.Badlogic.Gdx.Input.Keys.NUMPAD_5)) {
Offset.X = 0;
Offset.Y = 0;
}
float S = Dt * 230;
if (Gdx.Input.IsKeyPressed(Com.Badlogic.Gdx.Input.Keys.NUMPAD_4)) {
Velocity.X -= S;
}
if (Gdx.Input.IsKeyPressed(Com.Badlogic.Gdx.Input.Keys.NUMPAD_6)) {
Velocity.X += S;
}
if (Gdx.Input.IsKeyPressed(Com.Badlogic.Gdx.Input.Keys.NUMPAD_8)) {
Velocity.Y += S;
}
if (Gdx.Input.IsKeyPressed(Com.Badlogic.Gdx.Input.Keys.NUMPAD_2)) {
Velocity.Y -= S;
}
Offset.X += Velocity.X * Dt;
Offset.Y += Velocity.Y * Dt;
Velocity.X -= Velocity.X * Dt * 3;
Velocity.Y -= Velocity.Y * Dt * 3;
}
if (Dungeon.Game.GetState() != null && !Dungeon.Game.GetState().IsPaused()) {
T += Dt;
}
St = Math.Max(0, St - Dt * 1f);
PushAm = Math.Max(0, PushAm - Dt * 20);
Shake = Math.Max(0, Shake - Dt * 10);
if (Target != null && !NoMove) {
int X = (int) (Target.X + Target.W / 2);
int Y = (int) (Target.Y + Target.H / 2);
if (Target is Player && !((Player) Target).ToDeath) {
if (!Dungeon.Game.GetState().IsPaused() && !IgnoreMouse) {
MousePosition.X = Input.Instance.WorldMouse.X;
MousePosition.Y = Input.Instance.WorldMouse.Y;
}
} else {
Y += Target.H;
}
if (!Dungeon.Game.GetState().IsPaused()) {
CamPosition = CamPosition.Lerp(new Vector2(X, Y), Dt * Speed);
if (Target is Player) {
if (!Player.Instance.IsDead() && BurningKnight.Instance != null && !BurningKnight.Instance.GetState().Equals("unactive") && !BurningKnight.Instance.GetState().Equals("defeated")) {
CamPosition = CamPosition.Lerp(new Vector2(BurningKnight.Instance.X + BurningKnight.Instance.W / 2, BurningKnight.Instance.Y + BurningKnight.Instance.H / 2), Dt * Speed * 0.1f);
}
CamPosition = CamPosition.Lerp(new Vector2(MousePosition.X, MousePosition.Y), Dt * Speed * 0.25f);
Player P = (Player) Target;
if (P.Room != null && P.Room.LastNumEnemies > 0 && !(P.Room is FloatingRoom)) {
CamPosition = CamPosition.Lerp(new Vector2(P.Room.GetCenter().X * 16 + 8, P.Room.GetCenter().Y * 16 + 8), Dt * Speed * 0.5f);
}
}
}
if (Dungeon.Depth == -3) {
Room Room = Player.Instance.Room;
if (Room == null) {
Room = LastRoom;
} else {
LastRoom = Room;
}
if (Room != null) {
Game.Position.X = MathUtils.Clamp(Spawn.Instance.Room.Left * 16 + 16 + Display.GAME_WIDTH / 2, Spawn.Instance.Room.Right * 16 - Display.GAME_WIDTH / 2, CamPosition.X);
Game.Position.Y = MathUtils.Clamp(Spawn.Instance.Room.Top * 16 + 16 + Display.GAME_HEIGHT / 2 + 16, Spawn.Instance.Room.Bottom * 16 - Display.GAME_HEIGHT / 2 - 16, CamPosition.Y);
}
} else {
Game.Position.X = CamPosition.X + Offset.X;
Game.Position.Y = CamPosition.Y + Offset.Y;
}
Game.Update();
}
}
public static void ApplyShake() {
if (!NoMove) {
float Mx;
float My;
float Shake = St * St;
float Tt = T * 13;
if (Shake > 0.1f) {
Mx = Noise.Instance.Noise(Tt) * Shake;
My = (Noise.Instance.Noise(Tt + 1) * Shake);
Ma = (Noise.Instance.Noise(Tt + 2) * Shake * 0.5f);
} else {
Mx = 0;
My = 0;
Ma = 0;
}
if (Dungeon.Blood > 0) {
Ma += (Noise.Instance.Noise(T * 3 + 3) * Dungeon.Blood * 6);
}
if (PushAm > 0) {
float V = PushAm * PushAm * 0.3f;
Mx += (float) Math.Cos(PushA - Math.PI) * V;
My += (float) Math.Sin(PushA - Math.PI) * V;
}
Game.Position.Add(Mx, My, 0);
}
Game.Update();
}
public static void Shake(float Amount) {
St = Math.Min(Settings.Screenshake * 4, St + Amount * Settings.Screenshake * 0.5f);
}
public static void RemoveShake() {
Game.Position.X = CamPosition.X + Offset.X;
Game.Position.Y = CamPosition.Y + Offset.Y;
Game.Update();
}
public static void Follow(Entity Entity) {
Follow(Entity, true);
}
public static void Follow(Entity Entity, bool Jump) {
Target = Entity;
Speed = Entity is Player ? (4.5f) : 4;
if (Target == null) {
return;
}
if (Jump) {
float A = 1;
float B = 1 - A;
int X = (int) (((Target.X + Target.W / 2) * A + (Input.Instance.WorldMouse.X) * B));
int Y = (int) (((Target.Y + Target.H / 2) * A + (Input.Instance.WorldMouse.Y) * B));
CamPosition.Set(X, Y);
Game.Position.Set(X, Y, 0);
Game.Update();
}
}
}
}