using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class ParticleSpawner : Entity {
		private Vector2 Vel = new Vector2();
		private float Max;
		private float T;
		private float Last;
		private int I;

		public ParticleSpawner(float X, float Y) {
			this.X = X;
			this.Y = Y;
			this.AlwaysActive = true;
		}

		public override Void Init() {
			base.Init();
			this.Max = Random.NewFloat(1f, 1.5f);
			double A = Random.NewFloat(360);
			float Speed = 60 * Random.NewFloat(0.9f, 1.2f);
			this.Vel.X = (float) (Math.Cos(A) * Speed);
			this.Vel.Y = (float) (Math.Sin(A) * Speed);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.Last += Dt;

			if (this.Last >= 0.05f) {
				this.Last = 0;
				TinyParticle Explosion = new TinyParticle(X, Y);
				Explosion.Depth = 50 + this.I;
				Dungeon.Area.Add(Explosion);
				this.I++;
			} 

			this.T += Dt;

			if (this.T >= this.Max) {
				this.Done = true;
			} 

			this.X += this.Vel.X * Dt;
			this.Y += this.Vel.Y * Dt;
			this.Vel.Y -= 60f * Dt;
		}
	}
}
