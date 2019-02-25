using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.util;

namespace BurningKnight.core.game.fx {
	public class BackgroundFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		private float Size;
		private float Alpha;
		private float Val;
		private float Speed;

		public override Void Init() {
			Y = -32 - Random.NewFloat(32f);
			X = Random.NewFloat(0, Display.UI_WIDTH_MAX);
			Size = Random.NewFloat(2, 24) / 2f;
			Val = Random.NewFloat(0.7f, 1f);
			Speed = Size * 3f;
			Alpha = Random.NewFloat(0.3f, 0.8f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.Y += Speed * Dt;

			if (this.Y >= Display.UI_HEIGHT_MAX + 32) {
				this.Init();
			} 
		}

		public override Void Render() {
			Graphics.StartAlphaShape();
			Graphics.Shape.SetColor(this.Val, this.Val, this.Val, this.Alpha / 3);
			Graphics.Shape.Circle(this.X, this.Y, this.Size * 1.5f);
			Graphics.Shape.SetColor(this.Val, this.Val, this.Val, this.Alpha);
			Graphics.Shape.Circle(this.X, this.Y, this.Size);
			Graphics.EndAlphaShape();
		}

		public BackgroundFx() {
			_Init();
		}
	}
}
