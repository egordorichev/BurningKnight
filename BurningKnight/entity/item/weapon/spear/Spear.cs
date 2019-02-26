using BurningKnight.entity.item.weapon.gun;
using BurningKnight.entity.item.weapon.sword;
using BurningKnight.game;
using BurningKnight.physics;

namespace BurningKnight.entity.item.weapon.spear {
	public class Spear : Sword {
		public Spear() {
			_Init();
			base();
		}

		protected void _Init() {
			{
				Knockback = 30f;
				UseTime = 0.3f;
				DelayA = 0;
				TimeA = 0f;
				TimeB = 0.15f;
				UseTime = TimeA + TimeB + TimeDelay + 0.02f;
				var Letter = "a";
				Description = Locale.Get("spear_desc");
				Name = Locale.Get("spear_" + Letter);
				Sprite = "item-spear " + Letter.ToUpperCase();
				Damage = 4;
				Region = Graphics.GetTexture(Sprite);
			}
		}

		public override void OnPickup() {
			base.OnPickup();
			Achievements.Unlock("UNLOCK_SPEAR");
		}

		public override void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			float Angle = 0;

			if (Owner != null) {
				var Aim = Owner.GetAim();
				var An = Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI / 2;
				An = Gun.AngleLerp(LastAngle, An, 0.15f, Owner != null && Owner.Freezed);
				LastAngle = An;
				var A = (float) Math.ToDegrees(LastAngle);
				Angle += A;
			}

			TextureRegion Sprite = GetSprite();
			var A = (float) Math.ToRadians(Angle);
			var An = Added / 8f;
			var Xx = X + W / 2 + (Flipped ? -W / 4 : W / 4) + Math.Cos(A + Math.PI / 2) * (An - Region.GetRegionHeight() / 2);
			var Yy = Y + (Ox == 0 ? H / 4 : H / 2) + Math.Sin(A + Math.PI / 2) * (An - Region.GetRegionHeight() / 2);
			RenderAt(Xx - (Flipped ? Sprite.GetRegionWidth() : 0), Yy, Back ? (Flipped ? -45 : 45) : Angle, Sprite.GetRegionWidth() / 2 + (Flipped ? Ox : -Ox), Oy, Flipped, false);

			if (Body != null) World.CheckLocked(Body).SetTransform(Xx, Yy, A);
		}
	}
}