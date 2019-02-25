using BurningKnight.core.entity.item.weapon.projectile;

namespace BurningKnight.core.entity.pattern {
	public class WeirdPattern : BulletPattern {
		private int Count;
		private float D;

		protected override Void DoLogic(BulletProjectile Bullet, int I) {
			I = Bullet.I;
			D = Math.Min(64, D + Gdx.Graphics.GetDeltaTime() * 2f);
			Count = Math.Max(Count, Bullets.Size());
			float T = this.T * 3f;
			float A = (float) (((float) I) / Count * Math.PI * 2) + (T);
			float P = (float) (Math.Cos(T * 0.05f * Math.PI * 2) * 0.5f + 0.5f);
			float D = (float) (Math.Cos((I * (1f / Count * 2) + T * 0.05f) * Math.PI * 2) * this.D * P + (1 - P) * 32);
			Bullet.X = (float) (Math.Cos(A) * D) + X;
			Bullet.Y = (float) (Math.Sin(A) * D) + Y;
		}
	}
}
