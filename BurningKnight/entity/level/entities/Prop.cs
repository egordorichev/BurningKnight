using BurningKnight.save;

namespace BurningKnight.entity.level.entities {
	public class Prop : SaveableEntity {
		protected TextureRegion Region;
		public string Sprite;

		public override void Init() {
			base.Init();

			if (Sprite != null) Region = Graphics.GetTexture(Sprite);

			if (Region != null) {
				W = Region.GetRegionWidth();
				H = Region.GetRegionHeight();
			}
		}

		public override void Render() {
			Graphics.Render(Region, this.X, this.Y);
		}

		public override void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, W, H);
		}
	}
}