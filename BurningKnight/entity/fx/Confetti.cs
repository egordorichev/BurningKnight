using BurningKnight.util;

namespace BurningKnight.entity.fx {
	public class Confetti : Entity {
		private float A;
		private float B;
		private float G;
		private float M = 1f;

		private float R;
		private float T;
		private bool Tran;
		private float Tt;
		private float Va;
		public Vector2 Vel = new Vector2();
		private float Z;

		public Confetti() {
			_Init();
		}

		protected void _Init() {
			{
				W = 8;
				H = 4;
				Z = 20;
				Depth = 1;
			}
		}

		public override void Init() {
			base.Init();
			Tran = Random.Chance(50);
			Color Color = ColorUtils.HSV_to_RGB(Random.NewFloat(360f), 100f, 100f);
			R = Color.R;
			G = Color.G;
			B = Color.B;
			T = Random.NewFloat(2);
			Va = Random.NewFloat(-1, 1) * 360f;
			A = Random.NewFloat(360);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt;
			Tt += Dt;
			A += Va * Dt;
			this.X += Vel.X * Dt;
			this.Y += Vel.Y * Dt;
			Z += Vel.Y * Dt;
			Vel.X -= Vel.X * Math.Min(1, Dt * 2.5f);

			if (Z > 0) {
				Vel.Y -= 1f * Dt * 60;
			}
			else {
				Vel.X = 0;
				Vel.Y = 0;
				M = Math.Max(0, M - Dt * 5);
			}


			if (Tt >= 5f || M == 0) Done = true;
		}

		public override void Render() {
			Graphics.StartAlphaShape();

			if (Tran) {
				Graphics.Shape.SetColor(R, G, B, 1);
				Graphics.Shape.Rect(this.X, this.Y, W / 2, H / 2, W, H, (float) Math.Cos(T * 1.5f) * M, (float) Math.Sin(T * 0.9f) * M, A * M);
			}
			else {
				Graphics.Shape.SetColor(R, G, B, 0.5f);
				Graphics.Shape.Rect(this.X, this.Y, W / 2, H / 2, W, H, (float) Math.Cos(T * 1.5f) * M, (float) Math.Sin(T * 0.9f) * M, A * M);
				Graphics.Shape.SetColor(R, G, B, 1);
				Graphics.Shape.Rect(this.X, this.Y, W / 2, H / 2, W, H, (float) Math.Cos(T * 1.5f) * M * 0.5f, (float) Math.Sin(T * 0.9f) * M * 0.5f, A * M);
			}


			Graphics.EndAlphaShape();
		}
	}
}