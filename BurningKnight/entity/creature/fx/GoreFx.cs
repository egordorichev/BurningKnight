using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.fx {
	public class GoreFx : Entity {
		private float A;
		private float Al = 1f;
		private Body Body;
		private float Hz;
		public bool Menu;
		private float StartX;
		private float T;

		public TextureRegion Texture;
		private float Va;
		private Point Vel;
		private float Wz;
		private float Z = 8;

		public GoreFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Init() {
			base.Init();
			StartX = X;
			Depth = -1;
			A = Random.NewFloat(360);
			Va = Random.NewFloat(-20f, 20f);
			var Hz = Random.NewFloat(-1f, 1f);
			var Wz = Random.NewFloat(-1f, 1f);
			Vel = new Point((Random.Chance(50) ? -1 : 1) * (Math.Abs(this.Wz) + 1f), 1.5f + Hz);
			this.Hz = Hz * 16f;
			this.Wz = Wz * 32f;

			if (!Menu) {
				Body = World.CreateSimpleCentredBody(this, 0, 0, Texture.GetRegionWidth(), Texture.GetRegionHeight(), BodyDef.BodyType.DynamicBody, false);
				World.CheckLocked(Body).SetTransform(this.X + Texture.GetRegionWidth() / 2, this.Y + Texture.GetRegionHeight() / 2, 0);
				Body.SetLinearVelocity(Vel.X, Vel.Y);
				Body.SetBullet(true);
			}
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt;

			if (Settings.Quality == 0 && T >= 5f) {
				Al -= Dt;

				if (Al <= 0) Done = true;
			}

			if (Body != null) {
				Vel.X = Body.GetLinearVelocity().X;
				Vel.Y = Body.GetLinearVelocity().Y;
			}

			A += Va * Dt * 60;
			this.X = MathUtils.Clamp(StartX - Wz, StartX + Wz, this.X + Vel.X * Dt * 60);
			Vel.Y -= Dt * 8f;

			if (!Menu) {
				Z = Math.Max(Hz, Z + Vel.Y * Dt * 60);
			}
			else {
				Z += Vel.Y * Dt * 60;

				if (this.Y + Z < 0) Done = true;
			}


			A += Va * Dt * 60;
			Va -= Va * Math.Min(1, Dt * 3);

			if (Math.Abs(Vel.X) <= 0.1f || Z == Hz) {
				Vel.X = 0;
				Va = 0;

				if (Body != null) Body = World.RemoveBody(Body);
			}

			if (Body != null) {
				Body.SetLinearVelocity(Vel.X, Vel.Y);
				World.CheckLocked(Body).SetTransform(this.X, this.Y + Z, 0);
			}
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			Graphics.Batch.SetColor(1, 1, 1, Al);
			Graphics.Render(Texture, this.X, this.Y + Z, A, Texture.GetRegionWidth() / 2, Texture.GetRegionHeight() / 2, false, false);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity != null) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}
	}
}