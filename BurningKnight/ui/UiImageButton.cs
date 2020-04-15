using BurningKnight.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiImageButton : UiButton {
		private string id;
		public float Size = 2;

		public string Id {
			get => id;

			set {
				id = value;

				if (id == null) {
					Region = null;
					return;
				}
				
				Region = CommonAse.Ui.GetSlice(id);
				Width = Region.Width * Size;
				Height = Region.Height * Size;
			}
		}

		public TextureRegion Region;

		public override void Render() {
			Graphics.Render(Region, Center, 0, Region.Center, new Vector2(scale * Size));
		}
	}
}