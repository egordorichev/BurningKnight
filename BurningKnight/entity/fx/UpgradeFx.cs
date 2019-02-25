using BurningKnight.util;

namespace BurningKnight.entity.fx {
	public class UpgradeFx : Entity {
		private static TextureRegion Small = Graphics.GetTexture("particle-upgrade_big");
		private static TextureRegion Huge = Graphics.GetTexture("particle-upgrade_huge");
		private bool Big;
		private float Grow;
		private float Scale;
		private bool Second;
		private float Speed;
		private float TarScale;
		private float Val;

		public UpgradeFx() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = 2;
				AlwaysRender = true;
				AlwaysActive = true;
			}
		}

		public override void Init() {
			base.Init();
			Grow = Random.NewFloat(3f, 6f);
			Speed = Random.NewFloat(12, 24);
			Val = Random.NewFloat(0.5f, 1f);
			Big = Random.Chance(30);
			TarScale = Random.NewFloat(0.5f, 1f);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			this.Y += Speed * Dt;

			if (Second) {
				Scale -= Dt / 2;
			}
			else {
				Scale += Dt * Grow;

				if (Scale >= TarScale) {
					Scale = TarScale;
					Second = true;
				}
			}


			if (Scale <= 0f) Done = true;
		}

		public override void Render() {
			TextureRegion Region = Big ? Huge : Small;
			Graphics.Batch.SetColor(Val, Val, Val, 1);
			Graphics.Render(Region, this.X, this.Y, 0, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Scale, Scale);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}