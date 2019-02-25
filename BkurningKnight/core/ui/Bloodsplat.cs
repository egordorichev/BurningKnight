using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.util;

namespace BurningKnight.core.ui {
	public class Bloodsplat : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		private static List<Animation.Frame> Blood = Animation.Make("fx-bloodsplat").GetFrames("idle");
		private float A;
		private float C;
		private TextureRegion Texture;
		private float Al;
		private float Alm;
		private float R;
		private bool Second;
		private float T;
		private bool Go;

		public override Void Init() {
			base.Init();
			this.C = Random.NewFloat(0.1f, 0.2f);
			R = Random.NewFloat(0.6f, 0.9f);
			Texture = Blood.Get(Random.NewInt(Blood.Size())).Frame;
			A = Random.NewFloat(360);
			Alm = Random.NewFloat(0.5f, 0.8f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (Dungeon.Game.GetState().IsPaused()) {
				this.Go = true;
			} 

			if (this.Go) {
				this.Al -= Dt * 10;

				if (this.Al <= 0) {
					this.Done = true;
				} 
			} 

			if (this.Second) {
				this.T += Dt;

				if (this.T >= 0.5f) {
					this.Al -= Dt * 2;

					if (this.Al <= 0) {
						this.Done = true;
					} 
				} 
			} else {
				this.Al += (this.Alm - this.Al) * Dt * 30;

				if (this.Al >= this.Alm - 0.05f) {
					this.Al = 1f;
					this.Second = true;
				} 
			}

		}

		public override Void Render() {
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Graphics.Batch.SetColor(R, this.C, this.C, this.Al);
			Graphics.Render(Texture, this.X, this.Y, this.A, Texture.GetRegionWidth() / 2, Texture.GetRegionHeight() / 2, false, false);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public Bloodsplat() {
			_Init();
		}
	}
}
