using BurningKnight.entity;

namespace BurningKnight.ui {
	public class UiImageButton : UiButton {
		private TextureRegion Region;

		public UiImageButton(string Texture, int X, int Y) {
			_Init();
			base(null, X, Y);
			Region = Graphics.GetTexture(Texture);
			W = Region.GetRegionWidth();
			H = Region.GetRegionHeight();
		}

		protected void _Init() {
			{
				Depth = 19;
				IsSelectable = false;
				AlwaysActive = true;
				AlwaysRender = true;
			}
		}

		public override void Render() {
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Graphics.Render(Region, this.X + W / 2, this.Y + H / 2, 0, W / 2, H / 2, false, false, Scale, Scale);
		}
	}
}