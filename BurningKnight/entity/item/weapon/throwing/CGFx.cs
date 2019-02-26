using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.fx;
using BurningKnight.entity.item.weapon.projectile;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.item.weapon.throwing {
	public class CGFx : Entity {
		private float A;

		private Body Body;
		private bool DoExplode;
		private TextureRegion Region = Graphics.GetTexture("item-confetti_grenade");
		private float T;
		private float Va;
		public Vector2 Vel = new Vector2();

		public CGFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				W = 6;
				H = 10;
			}
		}

		public override void Init() {
			base.Init();
			Body = World.CreateSimpleCentredBody(this, 0, 0, W, H, BodyDef.BodyType.DynamicBody, false, 0.8f);
			MassData Data = new MassData();
			Data.Mass = 0.1f;
			Body.SetMassData(Data);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, A);
			Body.SetLinearVelocity(Vel.X, Vel.Y);
			Va = Random.NewFloat(-1f, 1f) * 360f;
			PlaySfx("bomb_placed");
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Mob && !((Mob) Entity).IsFlying()) DoExplode = true;
		}

		public override void Update(float Dt) {
			if (DoExplode) {
				Explode();

				return;
			}

			base.Update(Dt);
			this.X = Body.GetPosition().X;
			this.Y = Body.GetPosition().Y;
			A += Va * Dt;
			Va -= Va * Math.Min(1, Dt * 3);
			Vel.X = Body.GetLinearVelocity().X;
			Vel.Y = Body.GetLinearVelocity().Y;
			Vel.X -= Vel.X * Math.Min(1, Dt * 3);
			Vel.Y -= Vel.Y * Math.Min(1, Dt * 3);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, (float) Math.ToRadians(A));
			Body.SetLinearVelocity(Vel.X, Vel.Y);
			T += Dt;

			if (T >= 5f) Explode();
		}

		private void Explode() {
			Done = true;
			PlaySfx("explosion");
			var X = this.X + W / 2;
			var Y = this.Y + H / 2;

			for (var I = 0; I < 50; I++) {
				var An = Random.NewFloat(Math.PI * 2);
				var Fx = new Confetti();
				Fx.X = X;
				Fx.Y = Y;
				var F = Random.NewFloat(40, 80f);
				Fx.Vel.X = (float) Math.Cos(An) * F;
				Fx.Vel.Y = (float) Math.Sin(An) * F;
				Dungeon.Area.Add(Fx);
			}

			var Explosion = new Explosion(X, Y);
			Dungeon.Area.Add(Explosion);
			var Smoke = new Smoke(X, Y + 8);
			Smoke.Delay = 0.2f;
			Dungeon.Area.Add(Smoke);

			for (var I = 0; I < 16; I++) {
				BulletProjectile Bullet = new NanoBullet();
				var F = 100f;
				var A = (float) (I * (Math.PI / 8));
				Bullet.Damage = 10;
				Bullet.X = (float) (this.X + Math.Cos(A) * 8);
				Bullet.Y = (float) (this.Y + Math.Sin(A) * 8);
				Bullet.Velocity.X = Math.Cos(A) * F;
				Bullet.Velocity.Y = Math.Sin(A) * F;
				Dungeon.Area.Add(Bullet);
			}
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			var Sx = (float) (Math.Cos(T * 16) / 2) + 1;
			var Sy = (float) (Math.Cos(T * 16 + Math.PI) / 2.5) + 1;
			Graphics.Render(Region, this.X, this.Y, A, W / 2, H / 2, false, false, Sx, Sy);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Creature) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}