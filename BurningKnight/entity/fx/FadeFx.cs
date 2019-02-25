using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.fx {
	public class FadeFx : Entity {
		private float A;
		private float Al;
		private float AVel;
		private float B;
		private float G;
		private bool Grow;
		private float R;
		private float Size;
		private float Target;
		public bool To;
		public Point Vel;

		public FadeFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 6;
			}
		}

		public override void Init() {
			base.Init();

			if (To) {
				Grow = true;
				Target = Random.NewFloat(8, 16);
			}
			else {
				Size = Random.NewFloat(3, 5);
			}


			Al = 1;
			R = 2;
			G = Random.NewFloat(0.6f, 1f) + 0.5f;
			B = 1f;
			A = Random.NewFloat(Math.PI * 2);
			AVel = Random.NewFloat(-1f, 1f);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (R > 0.3f) {
				R -= Dt * 1.5f;
				G -= Dt * 1.5f;
				B -= Dt * 1.5f;
			}

			A += AVel * Dt * 320;
			Vel.X -= Vel.X * Math.Min(1, Dt * 3);
			Vel.Y -= Vel.Y * Math.Min(1, Dt * 3);
			AVel -= AVel * Math.Min(1, Dt * 3);
			X += Vel.X * Dt;
			Y += Vel.Y * Dt;

			if (To) {
				if (Grow) {
					Size += Dt * 120;

					if (Size >= Target) {
						Size = Target;
						Grow = false;
					}
				}
				else {
					Size -= Dt * 10;
				}
			}
			else {
				Size += Dt * 10;
			}


			Al -= Dt * 0.7;

			if (Al <= 0 || Size <= 0) Done = true;
		}

		public override void Render() {
			Graphics.StartAlphaShape();
			Graphics.Shape.SetColor(R, G, B, Al * 0.5f);
			Graphics.Shape.Rect(X, Y, Size / 2, Size / 2, Size, Size, 1, 1, A);
			Graphics.Shape.SetColor(R, G, B, Al);
			Graphics.Shape.Rect(X, Y, Size / 2, Size / 2, Size, Size, 0.5f, 0.5f, A);
			Graphics.EndAlphaShape();
		}
	}
}