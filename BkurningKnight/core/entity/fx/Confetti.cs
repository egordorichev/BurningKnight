using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.fx {
	public class Confetti : Entity {
		protected void _Init() {
			{
				W = 8;
				H = 4;
				Z = 20;
				Depth = 1;
			}
		}

		private float R;
		private float G;
		private float B;
		private float T;
		public Vector2 Vel = new Vector2();
		private float A;
		private float Va;
		private float Tt;
		private float Z;
		private float M = 1f;
		private bool Tran;

		public override Void Init() {
			base.Init();
			Tran = Random.Chance(50);
			Color Color = ColorUtils.HSV_to_RGB(Random.NewFloat(360f), 100f, 100f);
			this.R = Color.R;
			this.G = Color.G;
			this.B = Color.B;
			this.T = Random.NewFloat(2);
			this.Va = Random.NewFloat(-1, 1) * 360f;
			this.A = Random.NewFloat(360);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;
			this.Tt += Dt;
			this.A += this.Va * Dt;
			this.X += this.Vel.X * Dt;
			this.Y += this.Vel.Y * Dt;
			this.Z += this.Vel.Y * Dt;
			this.Vel.X -= this.Vel.X * Math.Min(1, Dt * 2.5f);

			if (this.Z > 0) {
				this.Vel.Y -= 1f * Dt * 60;
			} else {
				this.Vel.X = 0;
				this.Vel.Y = 0;
				this.M = Math.Max(0, this.M - Dt * 5);
			}


			if (this.Tt >= 5f || this.M == 0) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			Graphics.StartAlphaShape();

			if (Tran) {
				Graphics.Shape.SetColor(this.R, this.G, this.B, 1);
				Graphics.Shape.Rect(this.X, this.Y, this.W / 2, this.H / 2, this.W, this.H, (float) Math.Cos(this.T * 1.5f) * this.M, (float) Math.Sin(this.T * 0.9f) * this.M, this.A * this.M);
			} else {
				Graphics.Shape.SetColor(this.R, this.G, this.B, 0.5f);
				Graphics.Shape.Rect(this.X, this.Y, this.W / 2, this.H / 2, this.W, this.H, (float) Math.Cos(this.T * 1.5f) * this.M, (float) Math.Sin(this.T * 0.9f) * this.M, this.A * this.M);
				Graphics.Shape.SetColor(this.R, this.G, this.B, 1);
				Graphics.Shape.Rect(this.X, this.Y, this.W / 2, this.H / 2, this.W, this.H, (float) Math.Cos(this.T * 1.5f) * this.M * 0.5f, (float) Math.Sin(this.T * 0.9f) * this.M * 0.5f, this.A * this.M);
			}


			Graphics.EndAlphaShape();
		}

		public Confetti() {
			_Init();
		}
	}
}
