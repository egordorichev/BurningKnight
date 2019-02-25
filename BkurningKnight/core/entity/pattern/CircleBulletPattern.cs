using BurningKnight.core.entity.item.weapon.projectile;

namespace BurningKnight.core.entity.pattern {
	public class CircleBulletPattern : BulletPattern {
		public float Radius = 16;
		public bool Grow;
		private float D;
		private int Count;

		protected override Void DoLogic(BulletProjectile Bullet, int I) {
			if (Grow) {
				D = Math.Min(Radius, D + Gdx.Graphics.GetDeltaTime() * 2);
			} else {
				D = Radius;
			}


			Count = Math.Max(Count, Bullets.Size());
			float A = (float) (((float) Bullet.I) / Count * Math.PI * 2) + T * 4;
			Bullet.X = (float) (Math.Cos(A) * D) + X;
			Bullet.Y = (float) (Math.Sin(A) * D) + Y;
		}
	}
}
