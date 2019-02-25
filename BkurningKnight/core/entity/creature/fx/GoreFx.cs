using BurningKnight.core.assets;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.fx {
	public class GoreFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public TextureRegion Texture;
		public bool Menu;
		private float A;
		private float Va;
		private Point Vel;
		private float Z = 8;
		private Body Body;
		private float Al = 1f;
		private float StartX;
		private float Hz;
		private float Wz;
		private float T;

		public override Void Init() {
			base.Init();
			this.StartX = X;
			this.Depth = -1;
			this.A = Random.NewFloat(360);
			this.Va = Random.NewFloat(-20f, 20f);
			float Hz = Random.NewFloat(-1f, 1f);
			float Wz = Random.NewFloat(-1f, 1f);
			this.Vel = new Point((Random.Chance(50) ? -1 : 1) * (Math.Abs(this.Wz) + 1f), 1.5f + Hz);
			this.Hz = Hz * 16f;
			this.Wz = Wz * 32f;

			if (!this.Menu) {
				this.Body = World.CreateSimpleCentredBody(this, 0, 0, this.Texture.GetRegionWidth(), this.Texture.GetRegionHeight(), BodyDef.BodyType.DynamicBody, false);
				World.CheckLocked(this.Body).SetTransform(this.X + this.Texture.GetRegionWidth() / 2, this.Y + this.Texture.GetRegionHeight() / 2, 0);
				this.Body.SetLinearVelocity(this.Vel.X, this.Vel.Y);
				this.Body.SetBullet(true);
			} 
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			T += Dt;

			if (Settings.Quality == 0 && this.T >= 5f) {
				this.Al -= Dt;

				if (this.Al <= 0) {
					this.Done = true;
				} 
			} 

			if (this.Body != null) {
				this.Vel.X = this.Body.GetLinearVelocity().X;
				this.Vel.Y = this.Body.GetLinearVelocity().Y;
			} 

			this.A += this.Va * Dt * 60;
			this.X = MathUtils.Clamp(this.StartX - this.Wz, this.StartX + this.Wz, this.X + this.Vel.X * Dt * 60);
			this.Vel.Y -= Dt * 8f;

			if (!Menu) {
				this.Z = Math.Max(Hz, this.Z + this.Vel.Y * Dt * 60);
			} else {
				this.Z += this.Vel.Y * Dt * 60;

				if (this.Y + this.Z < 0) {
					this.Done = true;
				} 
			}


			this.A += this.Va * Dt * 60;
			this.Va -= this.Va * Math.Min(1, Dt * 3);

			if (Math.Abs(this.Vel.X) <= 0.1f || this.Z == Hz) {
				this.Vel.X = 0;
				this.Va = 0;

				if (this.Body != null) {
					this.Body = World.RemoveBody(this.Body);
				} 
			} 

			if (this.Body != null) {
				this.Body.SetLinearVelocity(this.Vel.X, this.Vel.Y);
				World.CheckLocked(this.Body).SetTransform(this.X, this.Y + this.Z, 0);
			} 
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			Graphics.Batch.SetColor(1, 1, 1, this.Al);
			Graphics.Render(this.Texture, this.X, this.Y + this.Z, this.A, this.Texture.GetRegionWidth() / 2, this.Texture.GetRegionHeight() / 2, false, false);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity != null) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public GoreFx() {
			_Init();
		}
	}
}
