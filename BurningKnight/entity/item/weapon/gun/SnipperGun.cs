using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.gun {
	public class SnipperGun : Gun {
		protected void _Init() {
			{
				BulletSprite = "bullet-rekt";
			}
		}

		protected override BulletProjectile GetBullet() {
			return new RektBullet();
		}

		public override void Use() {
			if (ShowRedLine) return;

			if (!(Owner is Mob) && AmmoLeft <= 0 || Delay > 0) return;

			ShowRedLine = true;
			Tween.To(new Tween.Task(0, 1.5f) {

		public override void OnEnd() {
			RealUse();
			ShowRedLine = false;
		}
	});
}

protected void RealUse() {
base.Use();
}
protected override void SendBullets() {
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