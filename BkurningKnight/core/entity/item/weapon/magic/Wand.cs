using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon.magic {
	public class Wand : WeaponBase {
		protected void _Init() {
			{
				UseTime = 0.15f;
			}
		}

		protected float Speed = 120f;
		protected int Mana = 1;
		protected Color Color = Color.WHITE;
		protected Player Owner;
		protected TextureRegion Projectile;
		protected float LastAngle;
		protected float Sy = 1;
		protected float Sx = 1;

		public Void SetOwner(Creature Owner) {
			base.SetOwner(Owner);

			if (Owner is Player) {
				this.Owner = (Player) Owner;
			} 
		}

		public override Void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			if (this.Owner != null) {
				Point Aim = this.Owner.GetAim();
				float An = (float) (this.Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI / 2);
				An = Gun.AngleLerp(this.LastAngle, An, 0.15f, this.Owner != null && this.Owner.Freezed);
				this.LastAngle = An;
			} 

			TextureRegion S = this.GetSprite();
			this.RenderAt(X + W / 2, Y + H / 4, Back ? (Flipped ? -45 : 45) : (float) Math.ToDegrees(this.LastAngle), S.GetRegionWidth() / 2, 0, false, false, Sx, Sy);
		}

		public int GetManaUsage() {
			return Math.Max(1, this.Mana);
		}

		public override Void Use() {
			if (!CanBeUsed()) {
				return;
			} 

			int Mn = GetManaUsage();

			if (this.Owner.GetMana() < Mn) {
				return;
			} 

			this.Owner.PlaySfx("fireball_cast");
			base.Use();
			this.Owner.ModifyMana(-Mn);
			this.SendProjectiles();
			Point Aim = this.Owner.GetAim();
			float A = (float) (this.Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI * 2);
			Camera.Push(A, 16f);
			Sx = 2f;
			Sy = 0.5f;
			Tween.To(new Tween.Task(1f, 0.2f) {
				public override float GetValue() {
					return Sy;
				}

				public override Void SetValue(float Value) {
					Sy = Value;
				}
			});
			Tween.To(new Tween.Task(1f, 0.2f) {
				public override float GetValue() {
					return Sx;
				}

				public override Void SetValue(float Value) {
					Sx = Value;
				}
			});
		}

		protected Void SendProjectiles() {
			float A = (float) Math.ToDegrees(this.LastAngle);
			float H = this.Region.GetRegionHeight();
			double An = this.LastAngle + Math.PI / 2;
			this.Owner.Knockback.X -= Math.Cos(An) * 40f;
			this.Owner.Knockback.Y -= Math.Sin(An) * 40f;
			this.SpawnProjectile(this.Owner.X + this.Owner.W / 2 + H * (float) Math.Cos(An), this.Owner.Y + this.Owner.H / 4 + H * (float) Math.Sin(An), A + 90);
		}

		public Color GetColor() {
			return Color;
		}

		protected TextureRegion GetProjectile() {
			return null;
		}

		public Void SpawnProjectile(float X, float Y, float A) {
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

				protected override Void OnDeath() {
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

				public override Void Render() {
					Color Color = GetColor();
					Graphics.Batch.SetColor(Color.R, Color.G, Color.B, 0.4f);
					Graphics.Render(Projectile, this.X, this.Y, this.A, Projectile.GetRegionWidth() / 2, Projectile.GetRegionHeight() / 2, false, false, 2f, 2f);
					Graphics.Batch.SetColor(Color.R, Color.G, Color.B, 0.8f);
					Graphics.Render(Projectile, this.X, this.Y, this.A, Projectile.GetRegionWidth() / 2, Projectile.GetRegionHeight() / 2, false, false);
					Graphics.Batch.SetColor(1, 1, 1, 1);
				}

				public override Void Init() {
					base.Init();
					ManaUsed = Mana;
					Light = World.NewLight(32, new Color(1f, 1f, 1f, 1f), 64, X, Y);
				}

				public override Void Destroy() {
					base.Destroy();
					World.RemoveLight(Light);
				}

				public override Void Logic(float Dt) {
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
