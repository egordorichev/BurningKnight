using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.fx {
	public class CurseFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		private float Val;
		private float Angle;
		private float Size;
		private bool Second;
		private float Av;
		private float Al = 1;
		private float Yv;
		private float Speed;
		private float TarVal;

		public override Void Init() {
			base.Init();
			this.Angle = Random.NewFloat(360f);
			this.Av = Random.NewFloat(0.5f, 1f) * (Random.Chance(50) ? -1 : 1);
			this.TarVal = Random.NewFloat(0, 0.3f);
			this.Speed = Random.NewFloat(0.5f, 1f);
			this.Val = Random.NewFloat(0.9f, 1f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.Angle += this.Av * 10;
			this.Yv += Dt * 360 * 0.5f;
			this.Y += this.Yv * Dt * Speed;

			if (this.Second) {
				this.Al -= Dt;

				if (this.Al <= 0) {
					this.Done = true;
				} 
			} else {
				this.Size += Dt * 25f;

				if (this.Size >= 5f) {
					this.Second = true;
					this.Size = 5f;
				} 
			}


			this.Val += (this.TarVal - this.Val) * Dt * 3;
		}

		public override Void Render() {
			Graphics.StartAlphaShape();
			float S = this.Size / 2;
			Graphics.Shape.SetColor(Val, Val, Val, this.Al);
			Graphics.Shape.Rect(this.X, this.Y, S, S, this.Size, this.Size, 1, 1, this.Angle);
			Graphics.EndAlphaShape();
		}

		public CurseFx() {
			_Init();
		}
	}
}
