using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.item.weapon.projectile;

namespace BurningKnight.core.entity.item.weapon.gun {
	public class BadGun : Gun {
		protected void _Init() {
			{
				UseTime = 1f;
				Sprite = "item-gun_a";
				AmmoMax = 3;
				Accuracy = 7f;
			}
		}

		protected override BulletProjectile GetBullet() {
			return new NanoBullet();
		}

		public override Void Use() {
			this.Vel = Mob.ShotSpeedMod * 0.8f;
			base.Use();
		}

		public BadGun() {
			_Init();
		}
	}
}
