using BurningKnight.core.assets;
using BurningKnight.core.entity;

namespace BurningKnight.core.ui {
	public class UiImageButton : UiButton {
		protected void _Init() {
			{
				Depth = 19;
				IsSelectable = false;
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		private TextureRegion Region;

		public UiImageButton(string Texture, int X, int Y) {
			_Init();
			base(null, X, Y);
			Region = Graphics.GetTexture(Texture);
			W = Region.GetRegionWidth();
			H = Region.GetRegionHeight();
		}

		public override Void Render() {
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Graphics.Render(Region, this.X + W / 2, this.Y + H / 2, 0, W / 2, H / 2, false, false, this.Scale, this.Scale);
		}
	}
}
