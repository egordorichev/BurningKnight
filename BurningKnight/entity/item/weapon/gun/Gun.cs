namespace BurningKnight.entity.item.weapon.gun {
	public class Gun : WeaponBase {
		private static Vector2 Last = new Vector2();
		private static float ClosestFraction = 1.0f;

		private static RayCastCallback Callback = new RayCastCallback {

			public override float ReportRayFixture(Fixture Fixture,
			Vector2 Point,
			Vector2 Normal,
			float Fraction) {
			Object Data = Fixture.GetBody().GetUserData();
			if (Data == null || (Data is Door && !((Door) Data).IsOpen())) {
			if (Fraction < ClosestFraction) {
			ClosestFraction = Fraction;
			Last.X = Point.X;
			Last.Y = Point.Y;
		}

		protected void _Init() {
			{
				Sprite = "item-gun_a";
				Damage = 4;
				UseTime = 0.8f;
			}

			{
				UseTime = 0.2f;
			}
		}
	}

	return ClosestFraction;
}

};
public int AmmoMax = 12;
public bool ShowRedLine;
protected float Accuracy = -3f;
protected float Sx = 1f;
protected float Sy = 1f;
protected float Vel = 6f;
protected float TextureA;
protected bool Penetrates;
protected float Tw;
protected float Th;
protected bool S;
protected Point Origin = new Point(3, 1);
protected Point Hole = new Point(13, 6);
protected int AmmoLeft = 12;
protected float ChargeProgress;
protected float ReloadRate = 1;
protected bool Down;
protected float LastAngle;
protected bool Flipped;
protected bool LastFlip;
protected int UsedTime;
protected string BulletSprite;
private bool Pressed;
private bool Shown;
private float Time;
private bool Fired;
private bool Back = false;
private float ChargeA = 0;
private float Mod;
public int GetAmmoLeft() {
return AmmoLeft;
}
public void SetAmmoLeft(int I) {
this.AmmoLeft = I;
}
public bool IsReloading() {
return Pressed;
}
public override void UpdateInHands(float Dt) {
base.UpdateInHands(Dt);
if (AmmoLeft == 0 && this.ChargeProgress == 0 && (this.Owner is Mob)) {
Pressed = true;
}
if (AmmoLeft == 0 && Dungeon.Depth == -3 && !Shown) {
Shown = true;
}
if (AmmoLeft < AmmoMax && Pressed) {
if (this.ChargeProgress == 0 || this.Time == 0) {
this.Time = 1;
}
this.ChargeProgress += Dt * this.Time * ReloadRate;
if (this.ChargeProgress >= 1f) {
Pressed = false;
this.AmmoLeft = (this.AmmoMax);
this.OnAmmoAdded();
ChargeProgress = 0;
}
} else if (AmmoLeft < AmmoMax && this.Owner is Player && ((Player) this.Owner).StopT > 0.15f && !Down) {
this.ChargeProgress += Dt * ReloadRate * AmmoMax;
if (this.ChargeProgress >= 1f) {
AmmoLeft++;
this.OnAmmoAdded();
ChargeProgress = 0;
if (this.AmmoLeft == this.AmmoMax && this.Owner is Player) {
this.Owner.PlaySfx("magnet_start");
}
}
}
}
public override int GetValue() {
return this.AmmoLeft;
}
public override void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
if (!S) {
S = true;
this.Tw = this.GetSprite().GetRegionWidth();
this.Th = this.GetSprite().GetRegionHeight();
}
Point Aim = this.Owner.GetAim();
Flipped = Dungeon.Game.GetState().IsPaused() ? this.LastFlip : Aim.X < this.Owner.X + this.Owner.W / 2;
this.LastFlip = this.Flipped;
this.Flipped = Flipped;
float An = this.Owner.GetAngleTo(Aim.X, Aim.Y);
An = AngleLerp(this.LastAngle, An, 0.15f, this.Owner != null && this.Owner.Freezed);
this.LastAngle = An;
float A = (float) Math.ToDegrees(this.LastAngle);
if (Back) {
Flipped = !Flipped;
}
this.RenderAt(X + W / 2 + (Flipped ? -7 : 7), Y + H / 4 + this.Owner.Z, Back ? (Flipped ? 45 + 90 : 45) : (A + TextureA), this.Origin.X + this.Mod, this.Origin.Y, false, false, TextureA == 0 ? this.Sx : Flipped ? -this.Sx : this.Sx,
TextureA != 0 ? this.Sy : (Flipped) ? -this.Sy : this.Sy);
X = X + W / 2 + (Flipped ? -7 : 7);
Y = Y + H / 4 + this.Owner.Z;
float Xx = X + GetAimX(0, 0);
float Yy = Y + GetAimY(0, 0);
if (!Back && (this.ShowRedLine || (this.Owner is Player && ((Player) this.Owner).HasRedLine))) {
float D = Display.Width * 2;
ClosestFraction = 1f;
Last.X = -1;
float X2 = Xx + (float) Math.Cos(An) * D;
float Y2 = Yy + (float) Math.Sin(An) * D;
if (Xx != X2 || Yy != Y2) {
World.World.RayCast(Callback, Xx, Yy, X2, Y2);
}
float Tx;
float Ty;
if (Last.X != -1) {
Tx = Last.X;
Ty = Last.Y;
} else {
Tx = X2;
Ty = Y2;
}
Graphics.StartAlphaShape();
Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
Graphics.Shape.SetColor(1, 0, 0, 0.3f);
Graphics.Shape.RectLine(Xx, Yy, Tx, Ty, 3);
Graphics.Shape.Rect(Tx - 2.5f, Ty - 2.5f, 5, 5);
Graphics.Shape.SetColor(1, 0, 0, 0.7f);
Graphics.Shape.RectLine(Xx, Yy, Tx, Ty, 1);
Graphics.Shape.Rect(Tx - 1.5f, Ty - 1.5f, 3, 3);
Graphics.EndAlphaShape();
}
float Dt = Gdx.Graphics.GetDeltaTime();
if (this.ChargeProgress > 0 && this.ChargeProgress < 1f && this.ChargeA < 1 && (this.Owner is Mob || ((Player) this.Owner).StopT > 0.1f)) {
this.ChargeA += (1 - this.ChargeA) * Dt * 5;
this.Back = false;
} else if (this.ChargeA > 0) {
this.ChargeProgress = 0;
this.Back = (this.Owner is Mob || (this.AmmoLeft == this.AmmoMax));
this.ChargeA += -this.ChargeA * Dt * 5;
}
if (this.ChargeA <= 0.05f) {
if (!Fired) {
Fired = true;
}
} else {
Fired = false;
}
}
public static float AngleLerp(float A0, float A1, float T, bool Freezed) {
return Dungeon.Game.GetState().IsPaused() || (Freezed) ? A0 : A0 + ShortAngleDist(A0, A1) * (T * 60 * Gdx.Graphics.GetDeltaTime());
}
protected float GetAimX(float Ex, float Ey) {
return (float) Math.Cos(this.LastAngle) * (this.Hole.X - this.Origin.X + Ex) + (float) Math.Cos(this.LastAngle + (Flipped ? -Math.PI / 2 : Math.PI / 2)) * (this.Hole.Y - this.Origin.Y + Ey);
}
protected float GetAimY(float Ex, float Ey) {
return (float) Math.Sin(this.LastAngle) * (this.Hole.X - this.Origin.X + Ex) + (float) Math.Sin(this.LastAngle + (Flipped ? -Math.PI / 2 : Math.PI / 2)) * (this.Hole.Y - this.Origin.Y + Ey);
}
public static float ShortAngleDist(float A0, float A1) {
float Max = (float) (Math.PI * 2);
float Da = (A1 - A0) % Max;
return 2 * Da % Max - Da;
}
public override void Use() {
if (this.Delay > 0) {
return;
}
if (!(this.Owner is Archeologist)) {
if (this.AmmoLeft <= 0 || this.ChargeProgress != 0) {
if (this.ChargeProgress == 0 && (this.Owner is Mob)) {
Pressed = true;
}
}
if (this.AmmoLeft <= 0) {
if (this.Owner is Player) {
this.Owner.PlaySfx("no_ammo");
}
return;
}
}
this.AmmoLeft -= 1;
Tween.To(new Tween.Task(6, 0.05f) {
public override float GetValue() {
return Mod;
}
public override void SetValue(float Value) {
Mod = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(0, 0.1f) {
public override float GetValue() {
return Mod;
}
public override void SetValue(float Value) {
Mod = Value;
}
});
}
});
float T = this.UseTime;
this.UseTime = this.GetUseTimeGun();
base.Use();
this.UseTime = T;
this.UsedTime += 1;
GetSprite();
this.Owner.PlaySfx("gun_machinegun");
Point Aim = this.Owner.GetAim();
float A = (float) (this.Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI * 2);
if (Settings.Quality > 0) {
Shell Shell = new Shell();
float X = this.Owner.X + this.Owner.W / 2f;
float Y = this.Owner.Y + this.Owner.H / 4 + Region.GetRegionHeight() / 2 - 2;
Shell.X = X;
Shell.Y = Y - 10;
Shell.Vel = new Point((float) -Math.Cos(A) * 1f, 1.5f);
Dungeon.Area.Add(Shell);
}
this.Owner.Knockback.X -= Math.Cos(A) * 30f;
this.Owner.Knockback.Y -= Math.Sin(A) * 30f;
if (this.Owner is Player) {
Camera.Push(A, 16f);
}
Tween.To(new Tween.Task(0.5f, 0.1f) {
public override float GetValue() {
return Sx;
}
public override void SetValue(float Value) {
Sx = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(1f, 0.2f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return Sx;
}
public override void SetValue(float Value) {
Sx = Value;
}
});
}
});
Tween.To(new Tween.Task(1.4f, 0.1f) {
public override float GetValue() {
return Sy;
}
public override void SetValue(float Value) {
Sy = Value;
}
public override void OnEnd() {
Tween.To(new Tween.Task(1f, 0.2f, Tween.Type.BACK_OUT) {
public override float GetValue() {
return Sy;
}
public override void SetValue(float Value) {
Sy = Value;
}
});
}
});
this.SendBullets();
}
public override void Save(FileWriter Writer) {
base.Save(Writer);
Writer.WriteInt32(this.AmmoLeft);
}
public override void Load(FileReader Reader) {
base.Load(Reader);
this.AmmoLeft = Reader.ReadInt32();
}
public override bool CanBeUsed() {
return base.CanBeUsed() && AmmoLeft >= 0;
}
protected float GetUseTimeGun() {
return UseTime;
}
protected void SendBullets() {
Point Aim = this.Owner.GetAim();
float A = (float) (this.Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI * 2);
this.SendBullet((float) (A + Math.ToRadians(Random.NewFloat(-this.GetAccuracy(), this.GetAccuracy()))));
}
protected void SendBullet(float An) {
SendBullet(An, 0, 0);
}
public float GetAccuracy() {
return Math.Max(0, Accuracy - (this.Owner is Player ? ((Player) this.Owner).Accuracy : 0));
}
protected BulletProjectile GetBullet() {
return this.Owner == null || this.Owner is Mob ? new NanoBullet() : new SimpleBullet();
}
protected void SendBullet(float An, float Xx, float Yy) {
SendBullet(An, Xx, Yy, GetBullet());
}
protected void SendBullet(float An, float Xx, float Yy, BulletProjectile Bullet) {
float A = (float) Math.ToDegrees(An);
float X = this.Owner.X + this.Owner.W / 2 + (Flipped ? -7 : 7);
float Y = this.Owner.Y + this.Owner.H / 4 + this.Owner.Z;
Bullet.X = (X + this.GetAimX(Xx, Yy));
Bullet.Y = (Y + this.GetAimY(Xx, Yy));
Bullet.Damage = RollDamage();
Bullet.Owner = this.Owner;
Bullet.Bad = this.Owner is Mob;
Bullet.Penetrates = this.Penetrates;
Bullet.Gun = this;
this.ModifyBullet(Bullet);
float S = this.Vel * 60f;
Bullet.Velocity = new Point((float) Math.Cos(An) * S, (float) Math.Sin(An) * S);
Bullet.A = A;
Dungeon.Area.Add(Bullet);
}
protected void ModifyBullet(BulletProjectile Bullet) {
}
public void SetAccuracy(float Accuracy) {
this.Accuracy = Accuracy;
}
protected void OnAmmoAdded() {
}
public void RenderReload() {
if (this.ChargeA > 0) {
float X = this.Owner.X + this.Owner.W / 2;
float Y = this.Owner.Y + this.Owner.H;
Graphics.StartAlphaShape();
Graphics.Shape.SetColor(0, 0, 0, ChargeA);
Graphics.Shape.Rect(X - 9, Y - 1, 18, 3);
Graphics.Shape.SetColor(1, 1, 1, ChargeA);
Graphics.Shape.Rect(X - 8, Y, 16, 1);
float Xx = (Back ? 16 : (this.Owner is Mob ? this.ChargeProgress : (((float) (this.AmmoLeft) / this.AmmoMax) + this.ChargeProgress / 16f)) * 16) - 8 + X;
Graphics.Shape.SetColor(0, 0, 0, this.ChargeA);
Graphics.Shape.Rect(Xx - 2, Y - 2, 5, 5);
Graphics.Shape.SetColor(1, 1, 1, this.ChargeA);
Graphics.Shape.Rect(Xx - 1, Y - 1, 3, 3);
Graphics.EndAlphaShape();
}
}
protected string GetSfx() {
return "gun_6";
}
public override void Update(float Dt) {
base.Update(Dt);
if (this.Owner != null) {
this.Delay = Math.Max(0, this.Delay - Dt);
if (this.AmmoLeft > this.AmmoMax) {
this.AmmoLeft = (int) (this.AmmoMax);
}
}
}
public Gun() {
_Init();
}
}
}