using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class TinyParticle : Entity {
		private AnimationData Animation = Explosion.Boom.Get("particle");
		private float R;
		private float G;
		private float B;
		private float A;

		public TinyParticle(float X, float Y) {
			this.X = X;
			this.Y = Y;
			this.AlwaysActive = true;
		}

		public override Void Init() {
			base.Init();
			this.R = Random.NewFloat(0.8f, 1f);
			this.G = Random.NewFloat(0.8f, 1f);
			this.B = Random.NewFloat(0.8f, 1f);
			this.A = Random.NewFloat(360f);
		}

		public override Void Update(float Dt) {
			if (this.Animation.Update(Dt)) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			TextureRegion Region = this.Animation.GetCurrent().Frame;
			Graphics.Batch.SetColor(R, G, B, 1);
			float W = Region.GetRegionWidth() / 2;
			float H = Region.GetRegionHeight() / 2;
			this.Animation.Render(this.X - W, this.Y - H, false, false, W, H, this.A);
		}
	}
}
