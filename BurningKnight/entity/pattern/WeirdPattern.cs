using BurningKnight.entity.item.weapon.projectile;

namespace BurningKnight.entity.pattern {
	public class WeirdPattern : BulletPattern {
		private int Count;
		private float D;

		protected override void DoLogic(BulletProjectile Bullet, int I) {
			I = Bullet.I;
			D = Math.Min(64, D + Gdx.Graphics.GetDeltaTime() * 2f);
			Count = Math.Max(Count, Bullets.Size());
			var T = this.T * 3f;
			var A = (float) I / Count * Math.PI * 2 + T;
			var P = Math.Cos(T * 0.05f * Math.PI * 2) * 0.5f + 0.5f;
			var D = Math.Cos((I * (1f / Count * 2) + T * 0.05f) * Math.PI * 2) * this.D * P + (1 - P) * 32;
			Bullet.X = Math.Cos(A) * D + X;
			Bullet.Y = Math.Sin(A) * D + Y;
		}
	}
}