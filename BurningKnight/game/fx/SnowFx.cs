using BurningKnight.entity;
using BurningKnight.util;

namespace BurningKnight.game.fx {
	public class SnowFx : Entity {
		private static TextureRegion Region = Graphics.GetTexture("particle-big");
		private float Al = 1;
		private float Fl;
		private float Mod;
		private float OnTar;
		private float Rot;
		private float Scale;
		private float Sm;
		private float T;
		public float Tar;
		private float Vl;

		public SnowFx() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 30;
			}
		}

		public override void Init() {
			base.Init();
			X = Random.NewFloat(-Display.Width, Display.Width / 2) + Camera.Game.Position.X;
			Y = Camera.Game.Position.Y + Display.Height / 2;
			Scale = Random.NewFloat(0.2f, 0.8f);
			Al = Random.NewFloat(0.3f, 0.8f);
			Vl = Random.NewFloat(0.5f, 1f);
			T = Random.NewFloat(32);
			Mod = Random.NewFloat(0.2f, 1f);
			Fl = Random.NewFloat(0.5f, 1.5f);
			Rot = Random.NewFloat(-1, 1);
			Sm = Random.NewFloat(0.7f, 2f);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (this.Y <= Tar) {
				this.Y = Tar;
				OnTar += Dt;

				if (OnTar > 1f) {
					Al -= Dt * 3;

					if (Al <= 0) Done = true;
				}
			}
			else {
				T += Dt;
				this.X += Math.Cos(T * Fl) * Mod;
				this.Y -= Scale * Dt * 60 * Sm;
			}
		}

		public override void Render() {
			Graphics.Batch.SetColor(Vl, Vl, Vl, Al);
			Graphics.Render(Region, this.X, this.Y, T * Rot * 512, 3, 3, false, false, Scale, Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}