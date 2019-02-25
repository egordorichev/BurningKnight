using BurningKnight.core.assets;
using BurningKnight.core.entity.creature;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.fx {
	public class BloodDropFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 1;
			}
		}

		public Creature Owner;
		private float A;
		private float Tsz;
		private float Sz;
		private bool Second;
		private float RtSpd;
		private float Xv;
		private float Yv;
		private float G;
		private float B;
		private float R;

		public override Void Init() {
			base.Init();
			this.R = Random.NewFloat(0.7f, 1f);
			this.G = Random.NewFloat(0f, 0.15f);
			this.B = Random.NewFloat(0f, 0.15f);
			this.Tsz = Random.NewFloat(4, 6);
			this.A = Random.NewFloat(360);
			this.RtSpd = Random.NewFloat(45, 180) * (Random.Chance(50) ? -1 : 1);
			this.Xv = Random.NewFloat(-1, 1) * 10;
			this.X = this.Owner.W / 2 - 4 + Random.NewFloat(8) - this.Tsz / 2;
			this.Y = this.Owner.H - Random.NewFloat(this.Owner.H / 3) - 6;
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.A += this.RtSpd;
			this.Yv += Dt * 90;
			this.Y -= this.Yv * Dt;
			this.X += this.Xv * Dt;

			if (this.Second) {
				this.Sz -= Dt * 10;

				if (this.Sz <= 0) {
					this.Done = true;
				} 
			} else {
				this.Sz += (this.Tsz - this.Sz) * Dt * 20;

				if (this.Tsz - this.Sz <= 0.4f) {
					this.Second = true;
					this.Sz = this.Tsz;
				} 
			}

		}

		public override Void Render() {
			Graphics.StartAlphaShape();
			Graphics.Shape.SetColor(R, G, B, 0.6f);
			Graphics.Shape.Rect(this.X + this.Owner.X, this.Y + this.Owner.Y + this.Owner.Z, this.Sz / 2, this.Sz / 2, this.Sz, this.Sz, 1, 1, this.A);
			Graphics.EndAlphaShape();
		}

		public BloodDropFx() {
			_Init();
		}
	}
}
