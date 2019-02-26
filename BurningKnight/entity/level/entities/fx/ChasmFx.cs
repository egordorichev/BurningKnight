using BurningKnight.util;

namespace BurningKnight.entity.level.entities.fx {
	public class ChasmFx : Entity {
		private float A;
		private bool Grow = true;
		private float Rate;

		private float Size;
		private float V;
		private float Vx;
		private float Vy;

		public ChasmFx(float X, float Y) {
			_Init();
			this.X = X;
			this.Y = Y;
			V = Random.NewFloat(0.7f, 1f);
			Vx = Random.NewFloat(-3f, 3f);
			Vy = Random.NewFloat(2.5f, 3f);
			Size = Random.NewFloat(2f, 4f);
			Rate = 1 / Size;
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Grow) {
				A += Dt * 2;

				if (A >= 1f) {
					A = 1f;
					Grow = false;
				}
			}
			else {
				Size -= Dt / 3f * Rate;
				A -= Dt / 3f;

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
			Graphics.Shape.SetColor(V, V, V, A);
			Graphics.Shape.Rect(this.X, this.Y, S, S, Size, Size, 1, 1, 0);
			Graphics.Shape.End();
			Gdx.Gl.GlDisable(GL20.GL_BLEND);
			Graphics.Batch.Begin();
		}
	}
}