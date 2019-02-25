using BurningKnight.util;

namespace BurningKnight.entity.item {
	public class Smoke : Entity {
		private AnimationData Animation = Explosion.Boom.Get("smoke");
		private float B;
		public float Delay;
		private float G;
		private float R;

		public Smoke(float X, float Y) {
			_Init();
			this.X = X;
			this.Y = Y;
			AlwaysActive = true;
		}

		protected void _Init() {
			{
				Depth = 31;
			}
		}

		public override void Init() {
			base.Init();
			R = Random.NewFloat(0.8f, 1f);
			G = Random.NewFloat(0.8f, 1f);
			B = Random.NewFloat(0.8f, 1f);
		}

		public override void Update(float Dt) {
			if (Delay > 0) {
				Delay -= Dt;

				return;
			}

			if (Animation.Update(Dt)) Done = true;
		}

		public override void Render() {
			if (Delay > 0) return;

			TextureRegion Region = Animation.GetCurrent().Frame;
			float W = Region.GetRegionWidth() / 2;
			float H = Region.GetRegionHeight() / 2;
			Animation.Render(this.X - W, this.Y - H, false, false, W, H, 0);
		}
	}
}