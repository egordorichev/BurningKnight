using BurningKnight.core.entity.item.weapon.projectile;

namespace BurningKnight.core.entity.pattern {
	public class NanoPattern : BulletPattern {
		private int Count;

		protected override Void DoLogic(BulletProjectile Bullet, int I) {
			Count = Math.Max(Count, Bullets.Size());
			bool Fir = Bullet.I % 2 == 1;
			float A = (float) (((float) Bullet.I) / Count * Math.PI * 2) + (T * (Fir ? -3 : 3));
			float Radius = Math.Min(128f, T * 48f * (Fir ? 1 : 0.5f));
			Bullet.X = (float) (Math.Cos(A) * Radius) + X;
			Bullet.Y = (float) (Math.Sin(A) * Radius) + Y;

			if (I % 8 == 0) {
				Bullet.Done = true;
			} 
		}
	}
}
