using BurningKnight.util;

namespace BurningKnight.entity.item {
	public class ParticleSpawner : Entity {
		private int I;
		private float Last;
		private float Max;
		private float T;
		private Vector2 Vel = new Vector2();

		public ParticleSpawner(float X, float Y) {
			this.X = X;
			this.Y = Y;
			AlwaysActive = true;
		}

		public override void Init() {
			base.Init();
			Max = Random.NewFloat(1f, 1.5f);
			double A = Random.NewFloat(360);
			var Speed = 60 * Random.NewFloat(0.9f, 1.2f);
			Vel.X = Math.Cos(A) * Speed;
			Vel.Y = Math.Sin(A) * Speed;
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Last += Dt;

			if (Last >= 0.05f) {
				Last = 0;
				var Explosion = new TinyParticle(X, Y);
				Explosion.Depth = 50 + I;
				Dungeon.Area.Add(Explosion);
				I++;
			}

			T += Dt;

			if (T >= Max) Done = true;

			this.X += Vel.X * Dt;
			this.Y += Vel.Y * Dt;
			Vel.Y -= 60f * Dt;
		}
	}
}