using BurningKnight.util;

namespace BurningKnight.entity.level.entities.fx {
	public class LavaFx : Entity {
		private float A;
		private bool Grow = true;
		private float M;
		private float Rate;

		private float Size;
		private float V;
		private float Vx;
		private float Vy;

		public LavaFx(float X, float Y) {
			_Init();
			this.X = X;
			this.Y = Y;
			V = Random.NewFloat(0.7f, 1f);
			Vx = Random.NewFloat(-3f, 3f) * 2;
			Vy = Random.NewFloat(2.5f, 3f) * 2;
			Size = Random.NewFloat(2f, 4f);
			Rate = 1 / Size;
			M = Random.NewFloat(0, 0.8f);
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Grow) {
				A += Dt * 4;

				if (A >= 1f) {
					A = 1f;
					Grow = false;
				}
			}
			else {
				Size -= Dt / 2f * Rate;
				A -= Dt / 2f;

				if (A <= 0 || Size <= 0) {
					Done = true;

					return;
				}
			}


			this.Y += Dt * Vy;
			this.X -= Dt * Vx;
		}

		public override void Render() {
			Graphics.Batch.End();
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
			var S = Size / 2;
			Graphics.Shape.SetColor(V * 0.5f, 0, 0, A);
			Graphics.Shape.Rect(this.X, this.Y, S, S, Size, Size, 1, 1, 0);
			Graphics.Shape.End();
			Gdx.Gl.GlDisable(GL20.GL_BLEND);
			Graphics.Batch.Begin();
		}
	}
}