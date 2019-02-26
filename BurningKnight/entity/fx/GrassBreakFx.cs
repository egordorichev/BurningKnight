using BurningKnight.util;

namespace BurningKnight.entity.fx {
	public class GrassBreakFx : Entity {
		private float Angle;
		private float G;
		private bool Second;

		private float Size;
		private float TarSize;
		private Vector2 Vel = new Vector2();

		public GrassBreakFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Init() {
			base.Init();
			G = Random.NewFloat(0.7f, 1f);
			TarSize = Random.NewFloat(3f, 6f);
			Angle = Random.NewFloat(360);
			Vel.X = Random.NewFloat(-1f, 1f) * 40;
			Vel.Y = Random.NewFloat(-1f, 1f) * 40;
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			this.X += Vel.X * Dt;
			this.Y += Vel.Y * Dt;
			Vel.X -= Vel.X * Math.Min(1, Dt * 3);
			Vel.Y -= Vel.Y * Math.Min(1, Dt * 3);

			if (Second) {
				Size -= Dt * 3f;

				if (Size <= 0) Done = true;
			}
			else {
				Size += Dt * 48;

				if (Size >= TarSize) {
					Size = TarSize;
					Second = true;
				}
			}
		}

		public override void Render() {
			Graphics.StartAlphaShape();
			var S = Size;
			Graphics.Shape.SetColor(0.1f, G, 0.1f, 0.4f);
			Graphics.Shape.Rect(this.X, this.Y, S, S, Size, Size, 1, 1, Angle);
			Graphics.EndAlphaShape();
		}
	}
}