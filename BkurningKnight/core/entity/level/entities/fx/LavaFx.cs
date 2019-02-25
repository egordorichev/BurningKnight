using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.entities.fx {
	public class LavaFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		private float Size;
		private float A;
		private float Vx;
		private float Vy;
		private float Rate;
		private float V;
		private float M;
		private bool Grow = true;

		public LavaFx(float X, float Y) {
			_Init();
			this.X = X;
			this.Y = Y;
			this.V = Random.NewFloat(0.7f, 1f);
			this.Vx = Random.NewFloat(-3f, 3f) * 2;
			this.Vy = Random.NewFloat(2.5f, 3f) * 2;
			this.Size = Random.NewFloat(2f, 4f);
			this.Rate = 1 / this.Size;
			M = Random.NewFloat(0, 0.8f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (Grow) {
				this.A += Dt * 4;

				if (this.A >= 1f) {
					this.A = 1f;
					this.Grow = false;
				} 
			} else {
				this.Size -= Dt / 2f * this.Rate;
				this.A -= Dt / 2f;

				if (this.A <= 0 || this.Size <= 0) {
					this.Done = true;

					return;
				} 
			}


			this.Y += Dt * this.Vy;
			this.X -= Dt * this.Vx;
		}

		public override Void Render() {
			Graphics.Batch.End();
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
			float S = this.Size / 2;
			Graphics.Shape.SetColor(V * 0.5f, 0, 0, this.A);
			Graphics.Shape.Rect(this.X, this.Y, S, S, this.Size, this.Size, 1, 1, 0);
			Graphics.Shape.End();
			Gdx.Gl.GlDisable(GL20.GL_BLEND);
			Graphics.Batch.Begin();
		}
	}
}
