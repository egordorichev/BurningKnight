using BurningKnight.core.util;

namespace BurningKnight.core.entity.item {
	public class Smoke : Entity {
		protected void _Init() {
			{
				Depth = 31;
			}
		}

		private AnimationData Animation = Explosion.Boom.Get("smoke");
		public float Delay;
		private float R;
		private float G;
		private float B;

		public Smoke(float X, float Y) {
			_Init();
			this.X = X;
			this.Y = Y;
			this.AlwaysActive = true;
		}

		public override Void Init() {
			base.Init();
			this.R = Random.NewFloat(0.8f, 1f);
			this.G = Random.NewFloat(0.8f, 1f);
			this.B = Random.NewFloat(0.8f, 1f);
		}

		public override Void Update(float Dt) {
			if (this.Delay > 0) {
				this.Delay -= Dt;

				return;
			} 

			if (this.Animation.Update(Dt)) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			if (this.Delay > 0) {
				return;
			} 

			TextureRegion Region = this.Animation.GetCurrent().Frame;
			float W = Region.GetRegionWidth() / 2;
			float H = Region.GetRegionHeight() / 2;
			this.Animation.Render(this.X - W, this.Y - H, false, false, W, H, 0);
		}
	}
}
