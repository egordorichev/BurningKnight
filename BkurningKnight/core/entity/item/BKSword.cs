using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.entity.fx;
using BurningKnight.core.entity.item.weapon.gun;
using BurningKnight.core.entity.item.weapon.sword;
using BurningKnight.core.game;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item {
	public class BKSword : SlashSword {
		protected void _Init() {
			{
				Name = Locale.Get("bk_sword");
				Description = Locale.Get("bk_sword_desc");
				Sprite = "item-bk_sword";
				Damage = 15;
				Penetrates = true;
			}
		}

		private float Last;

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Delay > 0) {
				Last += Gdx.Graphics.GetDeltaTime();
			} else {
				Last = 0;
			}


			if (Last >= 0.01f) {
				Last = 0;
				float Angle = Added;

				if (this.Owner != null) {
					Point Aim = this.Owner.GetAim();
					float An = (float) (this.Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI);
					An = Gun.AngleLerp(this.LastAngle, An, 0.15f, this.Owner != null && this.Owner.Freezed);
					this.LastAngle = An;
					float A = (float) Math.ToDegrees(this.LastAngle);
					Angle += (this.Owner.IsFlipped() ? A : -A);
					Angle = this.Owner.IsFlipped() ? Angle : 180 - Angle;
				} 

				TextureRegion Sprite = this.GetSprite();
				float Xx = X + W / 2 + (this.Owner.IsFlipped() ? 0 : W / 4) + Move;
				float Yy = Y + H / 4 + MoveY;
				BKSFx Fx = new BKSFx();
				Fx.Depth = this.Owner.Depth - 1;
				Fx.X = Xx - (this.Owner.IsFlipped() ? Sprite.GetRegionWidth() / 2 : 0);
				Fx.Y = Yy;
				Fx.A = Angle;
				Fx.Region = Sprite;
				Fx.Ox = Sprite.GetRegionWidth() / 2 + this.Ox;
				Fx.Oy = Oy;
				Fx.Flipped = this.Owner.IsFlipped();
			} 
		}

		public override Void OnHit(Creature Creature) {
			base.OnHit(Creature);
			Creature.AddBuff(new BurningBuff());
		}

		public override Void OnPickup() {
			base.OnPickup();
			Achievements.Unlock(Achievements.UNLOCK_SWORD_ORBITAL);
		}

		public BKSword() {
			_Init();
		}
	}
}
