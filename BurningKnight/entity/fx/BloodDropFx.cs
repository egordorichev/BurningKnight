using BurningKnight.entity.creature;
using BurningKnight.util;

namespace BurningKnight.entity.fx {
	public class BloodDropFx : Entity {
		private float A;
		private float B;
		private float G;

		public Creature Owner;
		private float R;
		private float RtSpd;
		private bool Second;
		private float Sz;
		private float Tsz;
		private float Xv;
		private float Yv;

		public BloodDropFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 1;
			}
		}

		public override void Init() {
			base.Init();
			R = Random.NewFloat(0.7f, 1f);
			G = Random.NewFloat(0f, 0.15f);
			B = Random.NewFloat(0f, 0.15f);
			Tsz = Random.NewFloat(4, 6);
			A = Random.NewFloat(360);
			RtSpd = Random.NewFloat(45, 180) * (Random.Chance(50) ? -1 : 1);
			Xv = Random.NewFloat(-1, 1) * 10;
			this.X = Owner.W / 2 - 4 + Random.NewFloat(8) - Tsz / 2;
			this.Y = Owner.H - Random.NewFloat(Owner.H / 3) - 6;
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			A += RtSpd;
			Yv += Dt * 90;
			this.Y -= Yv * Dt;
			this.X += Xv * Dt;

			if (Second) {
				Sz -= Dt * 10;

				if (Sz <= 0) Done = true;
			}
			else {
				Sz += (Tsz - Sz) * Dt * 20;

				if (Tsz - Sz <= 0.4f) {
					Second = true;
					Sz = Tsz;
				}
			}
		}

		public override void Render() {
			Graphics.StartAlphaShape();
			Graphics.Shape.SetColor(R, G, B, 0.6f);
			Graphics.Shape.Rect(this.X + Owner.X, this.Y + Owner.Y + Owner.Z, Sz / 2, Sz / 2, Sz, Sz, 1, 1, A);
			Graphics.EndAlphaShape();
		}
	}
}