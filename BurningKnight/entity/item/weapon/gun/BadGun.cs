using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item.weapon.projectile;

namespace BurningKnight.entity.item.weapon.gun {
	public class BadGun : Gun {
		public BadGun() {
			_Init();
		}

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

		public override void Use() {
			this.Vel = Mob.ShotSpeedMod * 0.8f;
			base.Use();
		}
	}
}