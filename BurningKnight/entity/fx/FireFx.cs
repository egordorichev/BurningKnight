using BurningKnight.entity.level;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.fx {
	public class FireFx : Entity {
		private static TextureRegion Region = Graphics.GetTexture("particle-big");
		private float A;
		private float Al = 0.9f;
		private float Av;
		private float B;
		private float G;
		private float Last;
		private float Min;
		private float R;
		private float Scale;
		private bool Second;
		public Point Vel = new Point();

		public FireFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 1;
			}
		}

		public override void Init() {
			base.Init();
			Min = Random.NewFloat(0.1f, 0.2f);
			G = 1f;
			R = 1f;
			B = 1f;
			A = Random.NewFloat(360);
			Av = Random.NewFloat(0.5f, 1f) * (Random.Chance(50) ? -1 : 1);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Last += Dt;

			if (Last >= 0.2f) {
				Last = 0;
				Dungeon.Level.SetOnFire(Level.ToIndex(Math.Round(this.X / 16), Math.Round(this.Y / 16)), true);
			}

			B = Math.Max(0f, B - Dt * 2f);
			G = Math.Max(Min, G - Dt);
			Scale += Scale >= 0.8f ? Scale * Dt * 0.5f : Dt * 5;

			if (Second) {
				Al -= Dt;

				if (Al <= 0f) Done = true;
			}
			else {
				if (Scale >= 1f) Second = true;
			}


			A += Av * Dt * 360 * 3;
			this.X += Vel.X * Dt;
			this.Y += Vel.Y * Dt;
			Vel.Mul(0.985f);
		}

		public override void Render() {
			Graphics.Batch.SetColor(R, G, B, Al / 3);
			Graphics.Render(Region, this.X, this.Y, A, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Scale * 2, Scale * 2);
			Graphics.Batch.SetColor(R, G, B, Al);
			Graphics.Render(Region, this.X, this.Y, A, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Scale, Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}