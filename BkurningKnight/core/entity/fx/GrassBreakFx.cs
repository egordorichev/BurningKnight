using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.fx {
	public class GrassBreakFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
			}
		}

		private float Size;
		private float TarSize;
		private bool Second;
		private Vector2 Vel = new Vector2();
		private float G;
		private float Angle;

		public override Void Init() {
			base.Init();
			G = Random.NewFloat(0.7f, 1f);
			TarSize = Random.NewFloat(3f, 6f);
			Angle = Random.NewFloat(360);
			Vel.X = Random.NewFloat(-1f, 1f) * 40;
			Vel.Y = Random.NewFloat(-1f, 1f) * 40;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.X += this.Vel.X * Dt;
			this.Y += this.Vel.Y * Dt;
			this.Vel.X -= this.Vel.X * Math.Min(1, Dt * 3);
			this.Vel.Y -= this.Vel.Y * Math.Min(1, Dt * 3);

			if (this.Second) {
				this.Size -= Dt * 3f;

				if (this.Size <= 0) {
					this.Done = true;
				} 
			} else {
				this.Size += Dt * 48;

				if (this.Size >= this.TarSize) {
					this.Size = this.TarSize;
					this.Second = true;
				} 
			}

		}

		public override Void Render() {
			Graphics.StartAlphaShape();
			float S = this.Size;
			Graphics.Shape.SetColor(0.1f, G, 0.1f, 0.4f);
			Graphics.Shape.Rect(this.X, this.Y, S, S, this.Size, this.Size, 1, 1, this.Angle);
			Graphics.EndAlphaShape();
		}

		public GrassBreakFx() {
			_Init();
		}
	}
}
