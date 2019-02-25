using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.game;

namespace BurningKnight.entity.pattern {
	public class BulletPattern : Entity {
		public List<BulletProjectile> Bullets = new List<>();
		protected float T;
		public Vector2 Velocity = new Vector2();

		public BulletPattern() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 16;
			}
		}

		public static void Fire(BulletPattern Pattern, float X, float Y, float A, float Speed) {
			Pattern.X = X;
			Pattern.Y = Y;
			Pattern.Velocity.X = Math.Cos(A) * Speed;
			Pattern.Velocity.Y = Math.Sin(A) * Speed;
			Dungeon.Area.Add(Pattern);
		}

		public override void Init() {
			base.Init();

			if (Bullets.Size() > 0) UpdateBullets(0);
		}

		public override void Render() {
			base.Render();
			Bullets.Sort(Area.Comparator);
			RenderBullets();
		}

		public override void RenderShadow() {
			base.RenderShadow();
			RenderShadows();
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Dt *= Mob.ShotSpeedMod;
			T += Dt;
			X += Velocity.X * Dt;
			Y += Velocity.Y * Dt;
			UpdateBullets(Dt);
			DoLogic(Dt);
		}

		public void AddBullet(BulletProjectile Bullet) {
			Bullets.Add(Bullet);
			Bullet.CanBeRemoved = true;
			Bullet.IgnoreBodyPos = true;
			Bullet.RenderCircle = false;
			Bullet.Pattern = this;
			Bullet.I = Bullets.Size() - 1;
			Bullet.Init();
		}

		public void RemoveBullet(BulletProjectile Bullet) {
			OnBulletRemove(Bullet);
			Bullets.Remove(Bullet);
		}

		protected void UpdateBullets(float Dt) {
			for (var I = Bullets.Size() - 1; I >= 0; I--) {
				BulletProjectile B = Bullets.Get(I);
				DoLogic(B, I);
				B.Update(Dt);

				if (B.Done || B.BrokeWeapon) {
					OnBulletRemove(B);
					B.Destroy();
					Bullets.Remove(B);
				}
			}

			if (Bullets.Size() == 0) Done = true;
		}

		protected void OnBulletRemove(BulletProjectile Bullet) {
		}

		protected void RenderBullets() {
			foreach (BulletProjectile B in Bullets) B.Render();
		}

		protected void RenderShadows() {
			foreach (BulletProjectile B in Bullets) B.RenderShadow();
		}

		protected void DoLogic(BulletProjectile Bullet, int I) {
		}

		protected void DoLogic(float Dt) {
		}

		public override void Destroy() {
			DestroyBullets();
		}

		protected void DestroyBullets() {
			foreach (BulletProjectile B in Bullets) {
				B.Done = true;
				B.Destroy();
			}

			Bullets.Clear();
		}
	}
}