using BurningKnight.entity.creature.mob;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.game;
using BurningKnight.game.input;
using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.axe {
	public class Axe : Weapon {
		private float Added;
		private float Ox;
		private float Oy;
		protected int Speed = 520;

		protected void _Init() {
			{
				Stackable = true;
				Penetrates = true;
				Ox = 0;
				Oy = 0;
				Auto = true;
				var Letter = "a";
				Name = Locale.Get("axe_" + Letter);
				Description = Locale.Get("axe_desc");
				Damage = 3;
				Penetrates = true;
				Sprite = "item-axe_" + Letter;
				Region = Graphics.GetTexture(Sprite);
			}
		}

		public override void OnPickup() {
			base.OnPickup();
			Achievements.Unlock("UNLOCK_AXE");
		}

		public override void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			var Angle = -Added;

			if (Owner != null) {
				if (Owner is Player) {
					var Dx = Owner.X + Owner.W / 2 - Input.Instance.WorldMouse.X - 8;
					var Dy = Owner.Y + Owner.H / 2 - Input.Instance.WorldMouse.Y - 8;
					var A = (float) Math.ToDegrees(Math.Atan2(Dy, Dx));
					Angle += Flipped ? A : -A;
					Angle = Flipped ? Angle : 180 - Angle;
				}
				else if (Owner is Mob && Added == 0) {
					var Mob = (Mob) Owner;

					if (Mob.Target != null && Mob.Saw && !Mob.IsDead()) {
						var Dx = Owner.X + Owner.W / 2 - Mob.Target.X - Mob.Target.W / 2;
						var Dy = Owner.Y + Owner.H / 2 - Mob.Target.Y - Mob.Target.H / 2;
						var A = (float) Math.ToDegrees(Math.Atan2(Dy, Dx));
						Angle += Flipped ? A : -A;
					}
					else {
						Angle += Flipped ? 0 : 180;
					}


					Angle = Flipped ? Angle : 180 - Angle;
				}
				else {
					Angle += Flipped ? 0 : 180;
					Angle = Flipped ? Angle : 180 - Angle;
				}
			}

			TextureRegion Sprite = GetSprite();
			var Xx = X + W / 2 + (Flipped ? -W / 4 : W / 4);
			var Yy = Y + (Ox == 0 ? H / 4 : H / 2);
			RenderAt(Xx, Yy, Back ? (Flipped ? -45 : 45) : Angle, Sprite.GetRegionWidth() / 2 + (Flipped ? Ox : -Ox), Oy, false, false, Flipped ? -1f : 1f, 1f);
		}

		protected bool CanBeConsumed() {
			return false;
		}

		public override void Use() {
			base.Use();
			var Self = this;
			var A = Owner.GetAngleTo(Input.Instance.WorldMouse.X, Input.Instance.WorldMouse.Y) - Math.PI;
			var S = 60f;
			var KnockbackMod = Owner.KnockbackMod;
			Owner.Knockback.X += Math.Cos(A) * S * KnockbackMod;
			Owner.Knockback.Y += Math.Sin(A) * S * KnockbackMod;
			Tween.To(new Tween.Task(130, 0.3f) {

		public override float GetValue() {
			return Added;
		}

		public override void SetValue(float Value) {
			Added = Value;
		}

		public override void OnEnd() {
			if (CanBeConsumed()) Count -= 1;

			Added = 0;
			Owner.PlaySfx("throw");
			var Fx = new AxeProjectile();
			Fx.Region = GetSprite();
			Fx.Type = Self.GetClass();
			Fx.X = Owner.X + (Owner.W - 16) / 2;
			Fx.Y = Owner.Y + Owner.H / 2;
			Fx.Speed = Speed;
			Fx.Owner = Owner;
			Fx.Damage = RollDamage();
			Fx.Penetrates = Penetrates;
			Fx.Axe = Self;
			Dungeon.Area.Add(Fx);
			EndUse();
		}
	});
}

protected override void CreateHitbox() {
}

public Axe() {
internal _Init();
}
}
}