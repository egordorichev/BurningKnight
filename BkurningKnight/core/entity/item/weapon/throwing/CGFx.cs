using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.mob;
using BurningKnight.core.entity.fx;
using BurningKnight.core.entity.item.weapon.projectile;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item.weapon.throwing {
	public class CGFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				W = 6;
				H = 10;
			}
		}

		private Body Body;
		private TextureRegion Region = Graphics.GetTexture("item-confetti_grenade");
		private float A;
		private float Va;
		public Vector2 Vel = new Vector2();
		private float T;
		private bool DoExplode;

		public override Void Init() {
			base.Init();
			this.Body = World.CreateSimpleCentredBody(this, 0, 0, W, H, BodyDef.BodyType.DynamicBody, false, 0.8f);
			MassData Data = new MassData();
			Data.Mass = 0.1f;
			this.Body.SetMassData(Data);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, this.A);
			this.Body.SetLinearVelocity(this.Vel.X, this.Vel.Y);
			this.Va = Random.NewFloat(-1f, 1f) * 360f;
			this.PlaySfx("bomb_placed");
		}

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Mob && !((Mob) Entity).IsFlying()) {
				this.DoExplode = true;
			} 
		}

		public override Void Update(float Dt) {
			if (this.DoExplode) {
				this.Explode();

				return;
			} 

			base.Update(Dt);
			this.X = this.Body.GetPosition().X;
			this.Y = this.Body.GetPosition().Y;
			this.A += this.Va * Dt;
			this.Va -= this.Va * Math.Min(1, Dt * 3);
			this.Vel.X = this.Body.GetLinearVelocity().X;
			this.Vel.Y = this.Body.GetLinearVelocity().Y;
			this.Vel.X -= this.Vel.X * Math.Min(1, Dt * 3);
			this.Vel.Y -= this.Vel.Y * Math.Min(1, Dt * 3);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, (float) Math.ToRadians(this.A));
			this.Body.SetLinearVelocity(this.Vel.X, this.Vel.Y);
			this.T += Dt;

			if (this.T >= 5f) {
				Explode();
			} 
		}

		private Void Explode() {
			this.Done = true;
			this.PlaySfx("explosion");
			float X = this.X + this.W / 2;
			float Y = this.Y + this.H / 2;

			for (int I = 0; I < 50; I++) {
				float An = Random.NewFloat((float) (Math.PI * 2));
				Confetti Fx = new Confetti();
				Fx.X = X;
				Fx.Y = Y;
				float F = Random.NewFloat(40, 80f);
				Fx.Vel.X = (float) Math.Cos(An) * F;
				Fx.Vel.Y = (float) Math.Sin(An) * F;
				Dungeon.Area.Add(Fx);
			}

			Explosion Explosion = new Explosion(X, Y);
			Dungeon.Area.Add(Explosion);
			Smoke Smoke = new Smoke(X, Y + 8);
			Smoke.Delay = 0.2f;
			Dungeon.Area.Add(Smoke);

			for (int I = 0; I < 16; I++) {
				BulletProjectile Bullet = new NanoBullet();
				float F = 100f;
				float A = (float) (I * (Math.PI / 8));
				Bullet.Damage = 10;
				Bullet.X = (float) (this.X + Math.Cos(A) * 8);
				Bullet.Y = (float) (this.Y + Math.Sin(A) * 8);
				Bullet.Velocity.X = (float) (Math.Cos(A) * F);
				Bullet.Velocity.Y = (float) (Math.Sin(A) * F);
				Dungeon.Area.Add(Bullet);
			}
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			float Sx = (float) (Math.Cos(this.T * 16) / 2) + 1;
			float Sy = (float) (Math.Cos(this.T * 16 + Math.PI) / 2.5) + 1;
			Graphics.Render(Region, this.X, this.Y, this.A, this.W / 2, this.H / 2, false, false, Sx, Sy);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Creature) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public CGFx() {
			_Init();
		}
	}
}
