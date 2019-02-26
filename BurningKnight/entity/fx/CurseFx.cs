using BurningKnight.util;

namespace BurningKnight.entity.fx {
	public class CurseFx : Entity {
		private float Al = 1;
		private float Angle;
		private float Av;
		private bool Second;
		private float Size;
		private float Speed;
		private float TarVal;

		private float Val;
		private float Yv;

		public CurseFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		public override void Init() {
			base.Init();
			Angle = Random.NewFloat(360f);
			Av = Random.NewFloat(0.5f, 1f) * (Random.Chance(50) ? -1 : 1);
			TarVal = Random.NewFloat(0, 0.3f);
			Speed = Random.NewFloat(0.5f, 1f);
			Val = Random.NewFloat(0.9f, 1f);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Angle += Av * 10;
			Yv += Dt * 360 * 0.5f;
			this.Y += Yv * Dt * Speed;

			if (Second) {
				Al -= Dt;

				if (Al <= 0) Done = true;
			}
			else {
				Size += Dt * 25f;

				if (Size >= 5f) {
					Second = true;
					Size = 5f;
				}
			}


			Val += (TarVal - Val) * Dt * 3;
		}

		public override void Render() {
			Graphics.StartAlphaShape();
			var S = Size / 2;
			Graphics.Shape.SetColor(Val, Val, Val, Al);
			Graphics.Shape.Rect(this.X, this.Y, S, S, Size, Size, 1, 1, Angle);
			Graphics.EndAlphaShape();
		}
	}
}