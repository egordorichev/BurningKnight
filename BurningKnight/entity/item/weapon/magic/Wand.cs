using BurningKnight.entity.creature;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.weapon.gun;
using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.magic {
	public class Wand : WeaponBase {
		protected Color Color = Color.WHITE;
		protected float LastAngle;
		protected int Mana = 1;
		protected Player Owner;
		protected TextureRegion Projectile;

		protected float Speed = 120f;
		protected float Sx = 1;
		protected float Sy = 1;

		protected void _Init() {
			{
				UseTime = 0.15f;
			}
		}

		public void SetOwner(Creature Owner) {
			base.SetOwner(Owner);

			if (Owner is Player) this.Owner = (Player) Owner;
		}

		public override void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			if (Owner != null) {
				var Aim = Owner.GetAim();
				var An = Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI / 2;
				An = Gun.AngleLerp(LastAngle, An, 0.15f, Owner != null && Owner.Freezed);
				LastAngle = An;
			}

			TextureRegion S = GetSprite();
			RenderAt(X + W / 2, Y + H / 4, Back ? (Flipped ? -45 : 45) : (float) Math.ToDegrees(LastAngle), S.GetRegionWidth() / 2, 0, false, false, Sx, Sy);
		}

		public int GetManaUsage() {
			return Math.Max(1, Mana);
		}

		public override void Use() {
			if (!CanBeUsed()) return;

			var Mn = GetManaUsage();

			if (Owner.GetMana() < Mn) return;

			Owner.PlaySfx("fireball_cast");
			base.Use();
			Owner.ModifyMana(-Mn);
			this.SendProjectiles();
			var Aim = Owner.GetAim();
			var A = Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI * 2;
			Camera.Push(A, 16f);
			Sx = 2f;
			Sy = 0.5f;
			Tween.To(new Tween.Task(1f, 0.2f) {

		public override float GetValue() {
			return Sy;
		}

		public override void SetValue(float Value) {
			Sy = Value;
		}
	});

	Tween.To(new Tween.Task(1f, 0.2f) {
	public override float GetValue() {
		return Sx;
	}

	public override void SetValue(float Value) {
		Sx = Value;
	}
	});
}

protected void SendProjectiles() {
internal float A = (float) Math.ToDegrees(this.LastAngle);
internal float H = this.Region.GetRegionHeight();

internal double An = this.LastAngle + Math.PI / 2;
	this.Owner.Knockback.X -= Math.Cos(An)* 40f;
this.Owner.Knockback.Y -= Math.Sin(An)* 40f;
this.
internal SpawnProjectile(this.Owner.X + this.Owner.W / 2 + H* (float) Math.Cos(An), this.Owner.Y + this.Owner.H / 4 + H* (float) Math.Sin(An), A + 90);
}

public Color GetColor() {
return Color;
}
protected TextureRegion GetProjectile() {
return null;
}
public void SpawnProjectile(float X, float Y, float A) {
if (Projectile == null) {
Projectile = GetProjectile();
}
int Mana = GetManaUsage();
BulletProjectile Missile = new BulletProjectile() {
protected void _Init() {
{
IgnoreArmor = true;
}
}
private int ManaUsed;
private bool Died;
private PointLight Light;
protected override void OnDeath() {
base.OnDeath();
if (Died) {
return;
}
Died = true;
int Weight = ManaUsed;
while (Weight > 0) {
ManaFx Fx = new ManaFx();
Fx.X = X;
Fx.Y = Y;
Fx.Half = Weight == 1;
Fx.Poof();
Weight -= Fx.Half ? 1 : 2;
Dungeon.Area.Add(Fx);
LevelSave.Add(Fx);
Fx.Body.SetLinearVelocity(new Vector2(-this.Velocity.X * 0.5f, -this.Velocity.Y * 0.5f));
}
}
public override void Render() {
Color Color = GetColor();
Graphics.Batch.SetColor(Color.R, Color.G, Color.B, 0.4f);
Graphics.Render(Projectile, this.X, this.Y, this.A, Projectile.GetRegionWidth() / 2, Projectile.GetRegionHeight() / 2, false, false, 2f, 2f);
Graphics.Batch.SetColor(Color.R, Color.G, Color.B, 0.8f);
Graphics.Render(Projectile, this.X, this.Y, this.A, Projectile.GetRegionWidth() / 2, Projectile.GetRegionHeight() / 2, false, false);
Graphics.Batch.SetColor(1, 1, 1, 1);
}
public override void Init() {
base.Init();
ManaUsed = Mana;
Light = World.NewLight(32, new Color(1f, 1f, 1f, 1f), 64, X, Y);
}
public override void Destroy() {
base.Destroy();
World.RemoveLight(Light);
}
public override void Logic(float Dt) {
base.Logic(Dt);
Light.SetPosition(X, Y);
this.Last += Dt;
if (this.Last > 0.05f) {
this.Last = 0;
RectFx Fx = new RectFx();
Fx.Depth = this.Depth;
Fx.X = this.X + Random.NewFloat(this.W) - this.W / 2;
Fx.Y = this.Y + Random.NewFloat(this.H) - this.H / 2;
Fx.W = 4;
Fx.H = 4;
Color Color = GetColor();
Fx.R = Color.R;
Fx.G = Color.G;
Fx.B = Color.B;
Dungeon.Area.Add(Fx);
}
World.CheckLocked(this.Body).SetTransform(this.X, this.Y, (float) Math.ToRadians(this.A));
}
public null() {
_Init();
}
};
Missile.Depth = 1;
Missile.Damage = this.RollDamage();
Missile.Owner = this.Owner;
Missile.X = X;
Missile.Y = Y - 3;
Missile.RectShape = true;
Missile.W = 6;
Missile.H = 6;
Missile.Rotates = true;
Missile.NoRotation = false;
double Ra = Math.ToRadians(A);
Missile.Velocity.X = (float) Math.Cos(Ra) * Speed;
Missile.Velocity.Y = (float) Math.Sin(Ra) * Speed;
Dungeon.Area.Add(Missile);
}
public override StringBuilder BuildInfo() {
StringBuilder Builder = base.BuildInfo();
Builder.Append("\n[blue]Uses ");
Builder.Append(GetManaUsage());
Builder.Append(" mana[gray]");
return Builder;
}
public Wand() {
_Init();
}
}
}