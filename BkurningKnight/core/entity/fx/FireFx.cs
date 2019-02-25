using BurningKnight.core.assets;
using BurningKnight.core.entity.level;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.fx {
	public class FireFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 1;
			}
		}

		private static TextureRegion Region = Graphics.GetTexture("particle-big");
		private float R;
		private float G;
		private float Scale;
		private float A;
		private float Av;
		public Point Vel = new Point();
		private bool Second;
		private float Last;
		private float B;
		private float Al = 0.9f;
		private float Min;

		public override Void Init() {
			base.Init();
			this.Min = Random.NewFloat(0.1f, 0.2f);
			this.G = 1f;
			this.R = 1f;
			this.B = 1f;
			this.A = Random.NewFloat(360);
			this.Av = Random.NewFloat(0.5f, 1f) * (Random.Chance(50) ? -1 : 1);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.Last += Dt;

			if (this.Last >= 0.2f) {
				this.Last = 0;
				Dungeon.Level.SetOnFire(Level.ToIndex(Math.Round(this.X / 16), Math.Round(this.Y / 16)), true);
			} 

			this.B = Math.Max(0f, B - Dt * 2f);
			this.G = Math.Max(this.Min, G - Dt);
			this.Scale += this.Scale >= 0.8f ? this.Scale * Dt * 0.5f : Dt * 5;

			if (this.Second) {
				this.Al -= Dt;

				if (this.Al <= 0f) {
					this.Done = true;
				} 
			} else {
				if (this.Scale >= 1f) {
					this.Second = true;
				} 
			}


			this.A += this.Av * Dt * 360 * 3;
			this.X += this.Vel.X * Dt;
			this.Y += this.Vel.Y * Dt;
			this.Vel.Mul(0.985f);
		}

		public override Void Render() {
			Graphics.Batch.SetColor(R, G, B, this.Al / 3);
			Graphics.Render(Region, this.X, this.Y, this.A, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Scale * 2, Scale * 2);
			Graphics.Batch.SetColor(R, G, B, this.Al);
			Graphics.Render(Region, this.X, this.Y, this.A, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Scale, Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public FireFx() {
			_Init();
		}
	}
}
