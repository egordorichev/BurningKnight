using BurningKnight.assets;
using BurningKnight.entity.item;
using BurningKnight.ui.str;
using Lens;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiDescriptionBanner : UiString {
		private string title;
		private int titleWidth;
		
		public UiDescriptionBanner() : base(Font.Small) {
			
		}

		public void Show(Item item) {
			title = item.Name;
			Label = item.Description;
			FinishTyping();

			var size = Font.Small.MeasureString(Label);

			Width = size.Width;
			Height = size.Height;

			CenterX = Display.UiWidth / 2f;
			Y = 64f;

			titleWidth = (int) Font.Medium.MeasureString(title).Width;
		}

		public override void Render() {
			if (title == null) {
				return;
			}
		
			Graphics.Print(title, Font.Medium, new Vector2((Display.UiWidth - titleWidth) / 2f, Y - 16));
			base.Render();
		}
	}
}