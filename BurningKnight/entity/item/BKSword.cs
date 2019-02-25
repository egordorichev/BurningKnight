using BurningKnight.entity.creature;
using BurningKnight.entity.creature.buff;
using BurningKnight.entity.fx;
using BurningKnight.entity.item.weapon.gun;
using BurningKnight.entity.item.weapon.sword;
using BurningKnight.game;

namespace BurningKnight.entity.item {
	public class BKSword : SlashSword {
		private float Last;

		public BKSword() {
			_Init();
		}

		protected void _Init() {
			{
				Name = Locale.Get("bk_sword");
				Description = Locale.Get("bk_sword_desc");
				Sprite = "item-bk_sword";
				Damage = 15;
				Penetrates = true;
			}
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Delay > 0)
				Last += Gdx.Graphics.GetDeltaTime();
			else
				Last = 0;


			if (Last >= 0.01f) {
				Last = 0;
				var Angle = Added;

				if (Owner != null) {
					var Aim = Owner.GetAim();
					var An = Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI;
					An = Gun.AngleLerp(LastAngle, An, 0.15f, Owner != null && Owner.Freezed);
					LastAngle = An;
					var A = (float) Math.ToDegrees(LastAngle);
					Angle += Owner.IsFlipped() ? A : -A;
					Angle = Owner.IsFlipped() ? Angle : 180 - Angle;
				}

				TextureRegion Sprite = GetSprite();
				var Xx = X + W / 2 + (Owner.IsFlipped() ? 0 : W / 4) + Move;
				var Yy = Y + H / 4 + MoveY;
				var Fx = new BKSFx();
				Fx.Depth = Owner.Depth - 1;
				Fx.X = Xx - (Owner.IsFlipped() ? Sprite.GetRegionWidth() / 2 : 0);
				Fx.Y = Yy;
				Fx.A = Angle;
				Fx.Region = Sprite;
				Fx.Ox = Sprite.GetRegionWidth() / 2 + Ox;
				Fx.Oy = Oy;
				Fx.Flipped = Owner.IsFlipped();
			}
		}

		public override void OnHit(Creature Creature) {
			base.OnHit(Creature);
			Creature.AddBuff(new BurningBuff());
		}

		public override void OnPickup() {
			base.OnPickup();
			Achievements.Unlock(Achievements.UNLOCK_SWORD_ORBITAL);
		}
	}
}