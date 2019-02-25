using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.game.fx {
	public class PixelFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		private static TextureRegion Region = Graphics.GetTexture("particle-rect");
		private float T;
		public float R;
		public float G;
		public float B;
		private float A = 1f;
		public Point Vel = new Point();

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.T += Dt;

			if (this.T <= 1f) {
				return;
			} 

			this.A -= Dt / 3;
			this.X += this.Vel.X * Dt;
			this.Y += this.Vel.Y * Dt;

			if (this.A <= 0) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			Graphics.Batch.SetColor(R, G, B, A);
			Graphics.Render(Region, this.X, this.Y, 0, 0, 0, false, false, 0.25f, 0.25f);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public PixelFx() {
			_Init();
		}
	}
}
