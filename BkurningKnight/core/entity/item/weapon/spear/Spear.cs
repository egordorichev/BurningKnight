using BurningKnight.core.assets;
using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.entity.item.weapon.sword;
using BurningKnight.core.game;
using BurningKnight.core.physics;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.weapon.spear {
	public class Spear : Sword {
		protected void _Init() {
			{
				Knockback = 30f;
				UseTime = 0.3f;
				DelayA = 0;
				TimeA = 0f;
				TimeB = 0.15f;
				UseTime = TimeA + TimeB + TimeDelay + 0.02f;
				string Letter = "a";
				Description = Locale.Get("spear_desc");
				Name = Locale.Get("spear_" + Letter);
				Sprite = "item-spear " + Letter.ToUpperCase();
				Damage = 4;
				Region = Graphics.GetTexture(Sprite);
			}
		}

		public Spear() {
			_Init();
			base();
		}

		public override Void OnPickup() {
			base.OnPickup();
			Achievements.Unlock("UNLOCK_SPEAR");
		}

		public override Void Render(float X, float Y, float W, float H, bool Flipped, bool Back) {
			float Angle = 0;

			if (this.Owner != null) {
				Point Aim = this.Owner.GetAim();
				float An = (float) (this.Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI / 2);
				An = Gun.AngleLerp(this.LastAngle, An, 0.15f, this.Owner != null && this.Owner.Freezed);
				this.LastAngle = An;
				float A = (float) Math.ToDegrees(this.LastAngle);
				Angle += A;
			} 

			TextureRegion Sprite = this.GetSprite();
			float A = (float) Math.ToRadians(Angle);
			float An = this.Added / 8f;
			float Xx = (float) (X + W / 2 + (Flipped ? -W / 4 : W / 4) + Math.Cos(A + Math.PI / 2) * (An - this.Region.GetRegionHeight() / 2));
			float Yy = (float) (Y + (this.Ox == 0 ? H / 4 : H / 2) + Math.Sin(A + Math.PI / 2) * (An - this.Region.GetRegionHeight() / 2));
			this.RenderAt(Xx - (Flipped ? Sprite.GetRegionWidth() : 0), Yy, Back ? (Flipped ? -45 : 45) : Angle, Sprite.GetRegionWidth() / 2 + (Flipped ? this.Ox : -this.Ox), this.Oy, Flipped, false);

			if (this.Body != null) {
				World.CheckLocked(this.Body).SetTransform(Xx, Yy, A);
			} 
		}
	}
}
