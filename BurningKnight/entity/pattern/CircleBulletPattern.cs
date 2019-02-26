using BurningKnight.entity.item.weapon.projectile;

namespace BurningKnight.entity.pattern {
	public class CircleBulletPattern : BulletPattern {
		private int Count;
		private float D;
		public bool Grow;
		public float Radius = 16;

		protected override void DoLogic(BulletProjectile Bullet, int I) {
			if (Grow)
				D = Math.Min(Radius, D + Gdx.Graphics.GetDeltaTime() * 2);
			else
				D = Radius;


			Count = Math.Max(Count, Bullets.Size());
			var A = (float) Bullet.I / Count * Math.PI * 2 + T * 4;
			Bullet.X = Math.Cos(A) * D + X;
			Bullet.Y = Math.Sin(A) * D + Y;
		}
	}
}