using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.game;

namespace BurningKnight.core.entity.pattern {
	public class BulletPattern : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 16;
			}
		}

		public List<BulletProjectile> Bullets = new List<>();
		public Vector2 Velocity = new Vector2();
		protected float T;

		public static Void Fire(BulletPattern Pattern, float X, float Y, float A, float Speed) {
			Pattern.X = X;
			Pattern.Y = Y;
			Pattern.Velocity.X = (float) (Math.Cos(A) * Speed);
			Pattern.Velocity.Y = (float) (Math.Sin(A) * Speed);
			Dungeon.Area.Add(Pattern);
		}

		public override Void Init() {
			base.Init();

			if (Bullets.Size() > 0) {
				UpdateBullets(0);
			} 
		}

		public override Void Render() {
			base.Render();
			Bullets.Sort(Area.Comparator);
			RenderBullets();
		}

		public override Void RenderShadow() {
			base.RenderShadow();
			RenderShadows();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			Dt *= Mob.ShotSpeedMod;
			T += Dt;
			X += Velocity.X * Dt;
			Y += Velocity.Y * Dt;
			UpdateBullets(Dt);
			DoLogic(Dt);
		}

		public Void AddBullet(BulletProjectile Bullet) {
			Bullets.Add(Bullet);
			Bullet.CanBeRemoved = true;
			Bullet.IgnoreBodyPos = true;
			Bullet.RenderCircle = false;
			Bullet.Pattern = this;
			Bullet.I = Bullets.Size() - 1;
			Bullet.Init();
		}

		public Void RemoveBullet(BulletProjectile Bullet) {
			OnBulletRemove(Bullet);
			Bullets.Remove(Bullet);
		}

		protected Void UpdateBullets(float Dt) {
			for (int I = Bullets.Size() - 1; I >= 0; I--) {
				BulletProjectile B = Bullets.Get(I);
				DoLogic(B, I);
				B.Update(Dt);

				if (B.Done || B.BrokeWeapon) {
					OnBulletRemove(B);
					B.Destroy();
					Bullets.Remove(B);
				} 
			}

			if (Bullets.Size() == 0) {
				Done = true;
			} 
		}

		protected Void OnBulletRemove(BulletProjectile Bullet) {

		}

		protected Void RenderBullets() {
			foreach (BulletProjectile B in Bullets) {
				B.Render();
			}
		}

		protected Void RenderShadows() {
			foreach (BulletProjectile B in Bullets) {
				B.RenderShadow();
			}
		}

		protected Void DoLogic(BulletProjectile Bullet, int I) {

		}

		protected Void DoLogic(float Dt) {

		}

		public override Void Destroy() {
			DestroyBullets();
		}

		protected Void DestroyBullets() {
			foreach (BulletProjectile B in Bullets) {
				B.Done = true;
				B.Destroy();
			}

			Bullets.Clear();
		}

		public BulletPattern() {
			_Init();
		}
	}
}
