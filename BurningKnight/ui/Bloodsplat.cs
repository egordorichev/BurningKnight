using BurningKnight.entity;
using BurningKnight.util;

namespace BurningKnight.ui {
	public class Bloodsplat : Entity {
		private static List<Animation.Frame> Blood = Animation.Make("fx-bloodsplat").GetFrames("idle");
		private float A;
		private float Al;
		private float Alm;
		private float C;
		private bool Go;
		private float R;
		private bool Second;
		private float T;
		private TextureRegion Texture;

		public Bloodsplat() {
			_Init();
		}

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		public override void Init() {
			base.Init();
			C = Random.NewFloat(0.1f, 0.2f);
			R = Random.NewFloat(0.6f, 0.9f);
			Texture = Blood.Get(Random.NewInt(Blood.Size())).Frame;
			A = Random.NewFloat(360);
			Alm = Random.NewFloat(0.5f, 0.8f);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Dungeon.Game.GetState().IsPaused()) Go = true;

			if (Go) {
				Al -= Dt * 10;

				if (Al <= 0) Done = true;
			}

			if (Second) {
				T += Dt;

				if (T >= 0.5f) {
					Al -= Dt * 2;

					if (Al <= 0) Done = true;
				}
			}
			else {
				Al += (Alm - Al) * Dt * 30;

				if (Al >= Alm - 0.05f) {
					Al = 1f;
					Second = true;
				}
			}
		}

		public override void Render() {
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Graphics.Batch.SetColor(R, C, C, Al);
			Graphics.Render(Texture, this.X, this.Y, A, Texture.GetRegionWidth() / 2, Texture.GetRegionHeight() / 2, false, false);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}