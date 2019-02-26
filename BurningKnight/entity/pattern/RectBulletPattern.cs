using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.util;

namespace BurningKnight.entity.pattern {
	public class RectBulletPattern : BulletPattern {
		private int Count;
		public float H = 32;
		public float W = 32;
		public float Xm = 16;
		public float Ym = 16;

		protected override void DoLogic(BulletProjectile Bullet, int I) {
			Count = Math.Max(Count, Bullets.Size());
			var A = (float) Bullet.I / Count * Math.PI * 2 + T * 4;
			Bullet.X = MathUtils.Clamp(-Xm, Xm, Math.Cos(A) * W) + X;
			Bullet.Y = MathUtils.Clamp(-Ym, Ym, Math.Sin(A) * H) + Y;
		}
	}
}