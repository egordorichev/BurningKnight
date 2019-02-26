using BurningKnight.entity;
using BurningKnight.util;

namespace BurningKnight.game.fx {
	public class BackgroundFx : Entity {
		private float Alpha;

		private float Size;
		private float Speed;
		private float Val;

		public BackgroundFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		public override void Init() {
			Y = -32 - Random.NewFloat(32f);
			X = Random.NewFloat(0, Display.UI_WIDTH_MAX);
			Size = Random.NewFloat(2, 24) / 2f;
			Val = Random.NewFloat(0.7f, 1f);
			Speed = Size * 3f;
			Alpha = Random.NewFloat(0.3f, 0.8f);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			this.Y += Speed * Dt;

			if (this.Y >= Display.UI_HEIGHT_MAX + 32) Init();
		}

		public override void Render() {
			Graphics.StartAlphaShape();
			Graphics.Shape.SetColor(Val, Val, Val, Alpha / 3);
			Graphics.Shape.Circle(this.X, this.Y, Size * 1.5f);
			Graphics.Shape.SetColor(Val, Val, Val, Alpha);
			Graphics.Shape.Circle(this.X, this.Y, Size);
			Graphics.EndAlphaShape();
		}
	}
}