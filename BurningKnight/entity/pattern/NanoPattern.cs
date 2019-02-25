using BurningKnight.entity.item.weapon.projectile;

namespace BurningKnight.entity.pattern {
	public class NanoPattern : BulletPattern {
		private int Count;

		protected override void DoLogic(BulletProjectile Bullet, int I) {
			Count = Math.Max(Count, Bullets.Size());
			var Fir = Bullet.I % 2 == 1;
			var A = (float) Bullet.I / Count * Math.PI * 2 + T * (Fir ? -3 : 3);
			float Radius = Math.Min(128f, T * 48f * (Fir ? 1 : 0.5f));
			Bullet.X = Math.Cos(A) * Radius + X;
			Bullet.Y = Math.Sin(A) * Radius + Y;

			if (I % 8 == 0) Bullet.Done = true;
		}
	}
}