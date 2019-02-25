using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.util;

namespace BurningKnight.core.game.fx {
	public class BackgroundFlameFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 1;
			}
		}

		private float G;
		private float T;
		private float Tt;
		private float Size = 1f;
		private float Range = 1;
		private float Angle;
		private float S;
		public float Oy;
		private float Speed;
		private float Ox;

		public override Void Init() {
			Speed = Random.NewFloat(0.7f, 2f);
			Y = -32 - Random.NewFloat(32f);
			X = Random.NewFloat(0, Display.GAME_WIDTH);
			G = Random.NewFloat(1);
			Oy = Y;
			Ox = X;
			Done = false;
			T = Random.NewFloat(1024);
			Tt = 0;
			Size = Random.NewFloat(8, 16);
			S = Random.NewFloat(3, 6);
			Range = Random.NewFloat(32, 64);
		}

		public override Void Update(float Dt) {
			this.T += Dt;
			this.Tt += Dt;
			this.Y = this.Oy + this.Tt * 48 * Speed;
			this.Size -= Dt * 4 * Speed;

			if (this.Size <= 0) {
				this.Init();
			} 

			this.X = Ox + (float) (Math.Cos(this.T) * this.Range);
			this.Angle = (float) Math.Cos(this.T * this.S) * 20;
		}

		public override Void Render() {
			Graphics.StartAlphaShape();
			float S = this.Size / 2;
			Graphics.Shape.SetColor(1, G, 0, 0.3f);
			Graphics.Shape.Rect(this.X, this.Y, S, S, this.Size, this.Size, 2, 2, this.Angle);
			Graphics.Shape.SetColor(1, G, 0, 0.7f);
			Graphics.Shape.Rect(this.X, this.Y, S, S, this.Size, this.Size, 1, 1, this.Angle);
			Graphics.EndAlphaShape();
		}

		public BackgroundFlameFx() {
			_Init();
		}
	}
}
