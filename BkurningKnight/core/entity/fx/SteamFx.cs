using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.fx {
	public class SteamFx : Entity {
		protected void _Init() {
			{
				T = Random.NewFloat(1024);
				Tt = 0;
				S = Random.NewFloat(3, 6);
				AlwaysActive = true;
			}
		}

		private float T;
		private float Tt;
		private float Size = 1f;
		private float Angle;
		private float S;
		private bool Second;
		private float Oy;
		public float Val;
		private float Speed;
		private float Al = 1;

		public override Void Init() {
			base.Init();
			Oy = Y;
			Speed = Random.NewFloat(1f, 2f);
			Val = Random.NewFloat(0.7f, 1f);
		}

		public override Void Update(float Dt) {
			this.T += Dt;
			this.Tt += Dt;
			this.Y = this.Oy + this.Tt * 15 * Speed;

			if (this.Second) {
				this.Al -= Dt * 0.5;

				if (this.Al <= 0) {
					this.Done = true;
				} 
			} else {
				this.Size += Dt * 80;

				if (this.Size >= 4) {
					this.Size = 4;
					this.Second = true;
				} 
			}


			this.Angle = (float) Math.Cos(this.T * this.S) * 20;

			if (this.Y - Oy > 64) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			Graphics.Batch.End();
			Gdx.Gl.GlEnable(GL20.GL_BLEND);
			Gdx.Gl.GlBlendFunc(GL20.GL_SRC_ALPHA, GL20.GL_ONE_MINUS_SRC_ALPHA);
			Graphics.Shape.SetProjectionMatrix(Camera.Game.Combined);
			Graphics.Shape.Begin(ShapeRenderer.ShapeType.Filled);
			float S = this.Size / 2;
			Graphics.Shape.SetColor(Val, Val, Val, Al);
			Graphics.Shape.Rect(this.X, this.Y, S, S, this.Size, this.Size, 1, 1, this.Angle);
			Graphics.Shape.End();
			Gdx.Gl.GlDisable(GL20.GL_BLEND);
			Graphics.Batch.Begin();
		}

		public SteamFx() {
			_Init();
		}
	}
}
