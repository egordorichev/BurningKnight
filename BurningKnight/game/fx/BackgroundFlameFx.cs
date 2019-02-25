using BurningKnight.entity;
using BurningKnight.util;

namespace BurningKnight.game.fx {
	public class BackgroundFlameFx : Entity {
		private float Angle;

		private float G;
		private float Ox;
		public float Oy;
		private float Range = 1;
		private float S;
		private float Size = 1f;
		private float Speed;
		private float T;
		private float Tt;

		public BackgroundFlameFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				Depth = 1;
			}
		}

		public override void Init() {
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

		public override void Update(float Dt) {
			T += Dt;
			Tt += Dt;
			this.Y = Oy + Tt * 48 * Speed;
			Size -= Dt * 4 * Speed;

			if (Size <= 0) Init();

			this.X = Ox + Math.Cos(T) * Range;
			Angle = (float) Math.Cos(T * S) * 20;
		}

		public override void Render() {
			Graphics.StartAlphaShape();
			var S = Size / 2;
			Graphics.Shape.SetColor(1, G, 0, 0.3f);
			Graphics.Shape.Rect(this.X, this.Y, S, S, Size, Size, 2, 2, Angle);
			Graphics.Shape.SetColor(1, G, 0, 0.7f);
			Graphics.Shape.Rect(this.X, this.Y, S, S, Size, Size, 1, 1, Angle);
			Graphics.EndAlphaShape();
		}
	}
}