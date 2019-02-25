using BurningKnight.util;

namespace BurningKnight.entity.fx {
	public class SteamFx : Entity {
		private float Al = 1;
		private float Angle;
		private float Oy;
		private float S;
		private bool Second;
		private float Size = 1f;
		private float Speed;

		private float T;
		private float Tt;
		public float Val;

		public SteamFx() {
			_Init();
		}

		protected void _Init() {
			{
				T = Random.NewFloat(1024);
				Tt = 0;
				S = Random.NewFloat(3, 6);
				AlwaysActive = true;
			}
		}

		public override void Init() {
			base.Init();
			Oy = Y;
			Speed = Random.NewFloat(1f, 2f);
			Val = Random.NewFloat(0.7f, 1f);
		}

		public override void Update(float Dt) {
			T += Dt;
			Tt += Dt;
			this.Y = Oy + Tt * 15 * Speed;

			if (Second) {
				Al -= Dt * 0.5;

				if (Al <= 0) Done = true;
			}
			else {
				Size += Dt * 80;

				if (Size >= 4) {
					Size = 4;
					Second = true;
				}
			}


			Angle = (float) Math.Cos(T * S) * 20;

			if (this.Y - Oy > 64) Done = true;
		}

		public override void Render() {
			Graphics.Batch.End();
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
			Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
			var S = Size / 2;
			Graphics.Shape.SetColor(Val, Val, Val, Al);
			Graphics.Shape.Rect(this.X, this.Y, S, S, Size, Size, 1, 1, Angle);
			Graphics.Shape.End();
			Gdx.Gl.GlDisable(GL20.GL_BLEND);
			Graphics.Batch.Begin();
		}
	}
}