using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.pattern {
	public class RectBulletPattern : BulletPattern {
		public float W = 32;
		public float H = 32;
		public float Xm = 16;
		public float Ym = 16;
		private int Count;

		protected override Void DoLogic(BulletProjectile Bullet, int I) {
			Count = Math.Max(Count, Bullets.Size());
			float A = (float) (((float) Bullet.I) / Count * Math.PI * 2) + T * 4;
			Bullet.X = MathUtils.Clamp(-Xm, Xm, (float) (Math.Cos(A) * W)) + X;
			Bullet.Y = MathUtils.Clamp(-Ym, Ym, (float) (Math.Sin(A) * H)) + Y;
		}
	}
}
