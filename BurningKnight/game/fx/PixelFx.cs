using BurningKnight.entity;
using BurningKnight.util.geometry;

namespace BurningKnight.game.fx {
	public class PixelFx : Entity {
		private static TextureRegion Region = Graphics.GetTexture("particle-rect");
		private float A = 1f;
		public float B;
		public float G;
		public float R;
		private float T;
		public Point Vel = new Point();

		public PixelFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			T += Dt;

			if (T <= 1f) return;

			A -= Dt / 3;
			this.X += Vel.X * Dt;
			this.Y += Vel.Y * Dt;

			if (A <= 0) Done = true;
		}

		public override void Render() {
			Graphics.Batch.SetColor(R, G, B, A);
			Graphics.Render(Region, this.X, this.Y, 0, 0, 0, false, false, 0.25f, 0.25f);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}