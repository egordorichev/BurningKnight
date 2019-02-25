using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.weapon.gun {
	public class SnipperGun : Gun {
		protected void _Init() {
			{
				BulletSprite = "bullet-rekt";
			}
		}

		protected override BulletProjectile GetBullet() {
			return new RektBullet();
		}

		public override Void Use() {
			if (ShowRedLine) {
				return;
			} 

			if ((!(this.Owner is Mob) && AmmoLeft <= 0) || this.Delay > 0) {
				return;
			} 

			ShowRedLine = true;
			Tween.To(new Tween.Task(0, 1.5f) {
				public override Void OnEnd() {
					RealUse();
					ShowRedLine = false;
				}
			});
		}

		protected Void RealUse() {
			base.Use();
		}

		protected override Void SendBullets() {
			Point Aim = this.Owner.GetAim();
			float A = (float) (this.Owner.GetAngleTo(Aim.X, Aim.Y) - Math.PI * 2);
			float V = 0.08f;
			this.SendBullet(A);
			this.SendBullet(A + V);
			this.SendBullet(A - V);
		}

		public SnipperGun() {
			_Init();
		}
	}
}
