using BurningKnight.core.assets;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.fx {
	public class ExplosionLeftOver : Entity {
		protected void _Init() {
			{
				Depth = -9;
			}
		}

		private static TextureRegion Region = Graphics.GetTexture("explosion-boom-00");
		private float A;

		public override Void Init() {
			base.Init();
			this.X -= Region.GetRegionWidth() / 2;
			this.Y -= Region.GetRegionHeight() / 2;
			this.A = Random.NewFloat(360f);
		}

		public override Void Render() {
			Graphics.Render(Region, this.X + Region.GetRegionWidth() / 2, this.Y + Region.GetRegionHeight() / 2, this.A, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false);
		}

		public ExplosionLeftOver() {
			_Init();
		}
	}
}
