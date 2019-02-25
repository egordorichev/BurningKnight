using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.fx {
	public class UpgradeFx : Entity {
		protected void _Init() {
			{
				Depth = 2;
				AlwaysRender = true;
				AlwaysActive = true;
			}
		}

		private static TextureRegion Small = Graphics.GetTexture("particle-upgrade_big");
		private static TextureRegion Huge = Graphics.GetTexture("particle-upgrade_huge");
		private bool Second;
		private float TarScale;
		private float Scale;
		private bool Big;
		private float Val;
		private float Speed;
		private float Grow;

		public override Void Init() {
			base.Init();
			Grow = Random.NewFloat(3f, 6f);
			Speed = Random.NewFloat(12, 24);
			Val = Random.NewFloat(0.5f, 1f);
			Big = Random.Chance(30);
			TarScale = Random.NewFloat(0.5f, 1f);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			this.Y += Speed * Dt;

			if (Second) {
				this.Scale -= Dt / 2;
			} else {
				this.Scale += Dt * Grow;

				if (this.Scale >= this.TarScale) {
					this.Scale = this.TarScale;
					this.Second = true;
				} 
			}


			if (this.Scale <= 0f) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			TextureRegion Region = (Big ? Huge : Small);
			Graphics.Batch.SetColor(Val, Val, Val, 1);
			Graphics.Render(Region, this.X, this.Y, 0, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Scale, Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		public UpgradeFx() {
			_Init();
		}
	}
}
