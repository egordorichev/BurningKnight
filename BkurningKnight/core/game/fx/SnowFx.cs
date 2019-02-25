using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.util;

namespace BurningKnight.core.game.fx {
	public class SnowFx : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 30;
			}
		}

		private static TextureRegion Region = Graphics.GetTexture("particle-big");
		private float Scale;
		private float T;
		private float Vl;
		private float Fl;
		private float Rot;
		private float Mod;
		private float Sm;
		public float Tar;
		private float Al = 1;
		private float OnTar;

		public override Void Init() {
			base.Init();
			X = Random.NewFloat(-Display.GAME_WIDTH, Display.GAME_WIDTH / 2) + Camera.Game.Position.X;
			Y = Camera.Game.Position.Y + Display.GAME_HEIGHT / 2;
			Scale = Random.NewFloat(0.2f, 0.8f);
			Al = Random.NewFloat(0.3f, 0.8f);
			Vl = Random.NewFloat(0.5f, 1f);
			T = Random.NewFloat(32);
			Mod = Random.NewFloat(0.2f, 1f);
			Fl = Random.NewFloat(0.5f, 1.5f);
			Rot = Random.NewFloat(-1, 1);
			Sm = Random.NewFloat(0.7f, 2f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Y <= this.Tar) {
				this.Y = this.Tar;
				this.OnTar += Dt;

				if (OnTar > 1f) {
					Al -= Dt * 3;

					if (Al <= 0) {
						Done = true;
					} 
				} 
			} else {
				this.T += Dt;
				this.X += Math.Cos(this.T * this.Fl) * this.Mod;
				this.Y -= this.Scale * Dt * 60 * Sm;
			}

		}

		public override Void Render() {
			Graphics.Batch.SetColor(Vl, Vl, Vl, Al);
			Graphics.Render(Region, this.X, this.Y, this.T * Rot * 512, 3, 3, false, false, Scale, Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public SnowFx() {
			_Init();
		}
	}
}
