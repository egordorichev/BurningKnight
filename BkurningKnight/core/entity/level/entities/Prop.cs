using BurningKnight.core.assets;

namespace BurningKnight.core.entity.level.entities {
	public class Prop : SaveableEntity {
		public string Sprite;
		protected TextureRegion Region;

		public override Void Init() {
			base.Init();

			if (this.Sprite != null) {
				Region = Graphics.GetTexture(Sprite);
			} 

			if (this.Region != null) {
				this.W = Region.GetRegionWidth();
				this.H = Region.GetRegionHeight();
			} 
		}

		public override Void Render() {
			Graphics.Render(Region, this.X, this.Y);
		}

		public override Void RenderShadow() {
			Graphics.Shadow(this.X, this.Y, this.W, this.H);
		}
	}
}
