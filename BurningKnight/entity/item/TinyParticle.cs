using BurningKnight.util;

namespace BurningKnight.entity.item {
	public class TinyParticle : Entity {
		private float A;
		private AnimationData Animation = Explosion.Boom.Get("particle");
		private float B;
		private float G;
		private float R;

		public TinyParticle(float X, float Y) {
			this.X = X;
			this.Y = Y;
			AlwaysActive = true;
		}

		public override void Init() {
			base.Init();
			R = Random.NewFloat(0.8f, 1f);
			G = Random.NewFloat(0.8f, 1f);
			B = Random.NewFloat(0.8f, 1f);
			A = Random.NewFloat(360f);
		}

		public override void Update(float Dt) {
			if (Animation.Update(Dt)) Done = true;
		}

		public override void Render() {
			TextureRegion Region = Animation.GetCurrent().Frame;
			Graphics.Batch.SetColor(R, G, B, 1);
			float W = Region.GetRegionWidth() / 2;
			float H = Region.GetRegionHeight() / 2;
			Animation.Render(this.X - W, this.Y - H, false, false, W, H, A);
		}
	}
}