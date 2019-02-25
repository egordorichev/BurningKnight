using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.game;
using BurningKnight.core.game.input;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.weapon.axe {
	public class Axe : Weapon {
		protected void _Init() {
			{
				Stackable = true;
				Penetrates = true;
				Ox = 0;
				Oy = 0;
				Auto = true;
				string Letter = "a";
				Name = Locale.Get("axe_" + Letter);
				Description = Locale.Get("axe_desc");
				Damage = 3;
				Penetrates = true;
				Sprite = "item-axe_" + Letter;
				Region = Graphics.GetTexture(Sprite);
			}
		}

		private float Added;
		private float Ox;
		private float Oy;
		protected int Speed = 520;

		public override Void OnPickup() {
			base.OnPickup();
			Achievements.Unlock("UNLOCK_AXE");
		}

		public override Void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			float Angle = -this.Added;

			if (this.Owner != null) {
				if (this.Owner is Player) {
					float Dx = this.Owner.X + this.Owner.W / 2 - Input.Instance.WorldMouse.X - 8;
					float Dy = this.Owner.Y + this.Owner.H / 2 - Input.Instance.WorldMouse.Y - 8;
					float A = (float) Math.ToDegrees(Math.Atan2(Dy, Dx));
					Angle += (Flipped ? A : -A);
					Angle = Flipped ? Angle : 180 - Angle;
				} else if (this.Owner is Mob && this.Added == 0) {
					Mob Mob = (Mob) this.Owner;

					if (Mob.Target != null && Mob.Saw && !Mob.IsDead()) {
						float Dx = this.Owner.X + this.Owner.W / 2 - Mob.Target.X - Mob.Target.W / 2;
						float Dy = this.Owner.Y + this.Owner.H / 2 - Mob.Target.Y - Mob.Target.H / 2;
						float A = (float) Math.ToDegrees(Math.Atan2(Dy, Dx));
						Angle += (Flipped ? A : -A);
					} else {
						Angle += (Flipped ? 0 : 180);
					}


					Angle = Flipped ? Angle : 180 - Angle;
				} else {
					Angle += (Flipped ? 0 : 180);
					Angle = Flipped ? Angle : 180 - Angle;
				}

			} 

			TextureRegion Sprite = this.GetSprite();
			float Xx = X + W / 2 + (Flipped ? -W / 4 : W / 4);
			float Yy = Y + (this.Ox == 0 ? H / 4 : H / 2);
			this.RenderAt(Xx, Yy, Back ? (Flipped ? -45 : 45) : Angle, Sprite.GetRegionWidth() / 2 + (Flipped ? this.Ox : -this.Ox), this.Oy, false, false, Flipped ? -1f : 1f, 1f);
		}

		protected bool CanBeConsumed() {
			return false;
		}

		public override Void Use() {
			base.Use();
			Axe Self = this;
			float A = (float) (this.Owner.GetAngleTo(Input.Instance.WorldMouse.X, Input.Instance.WorldMouse.Y) - Math.PI);
			float S = 60f;
			float KnockbackMod = Owner.KnockbackMod;
			this.Owner.Knockback.X += Math.Cos(A) * S * KnockbackMod;
			this.Owner.Knockback.Y += Math.Sin(A) * S * KnockbackMod;
			Tween.To(new Tween.Task(130, 0.3f) {
				public override float GetValue() {
					return Added;
				}

				public override Void SetValue(float Value) {
					Added = Value;
				}

				public override Void OnEnd() {
					if (CanBeConsumed()) {
						Count -= 1;
					} 

					Added = 0;
					Owner.PlaySfx("throw");
					AxeProjectile Fx = new AxeProjectile();
					Fx.Region = GetSprite();
					Fx.Type = Self.GetClass();
					Fx.X = Owner.X + (Owner.W - 16) / 2;
					Fx.Y = Owner.Y + (Owner.H) / 2;
					Fx.Speed = (int) (Speed);
					Fx.Owner = Owner;
					Fx.Damage = RollDamage();
					Fx.Penetrates = Penetrates;
					Fx.Axe = Self;
					Dungeon.Area.Add(Fx);
					EndUse();
				}
			});
		}

		protected override Void CreateHitbox() {

		}

		public Axe() {
			_Init();
		}
	}
}
