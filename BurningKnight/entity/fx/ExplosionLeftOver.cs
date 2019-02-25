using BurningKnight.util;

namespace BurningKnight.entity.fx {
	public class ExplosionLeftOver : Entity {
		private static TextureRegion Region = Graphics.GetTexture("explosion-boom-00");
		private float A;

		public ExplosionLeftOver() {
			_Init();
		}

		protected void _Init() {
			{
				Depth = -9;
			}
		}

		public override void Init() {
			base.Init();
			this.X -= Region.GetRegionWidth() / 2;
			this.Y -= Region.GetRegionHeight() / 2;
			A = Random.NewFloat(360f);
		}

		public override void Render() {
			Graphics.Render(Region, this.X + Region.GetRegionWidth() / 2, this.Y + Region.GetRegionHeight() / 2, A, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false);
		}
	}
}