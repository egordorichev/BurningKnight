using BurningKnight.assets;
using BurningKnight.entity.item;
using BurningKnight.ui.str;
using Lens;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Timer = Lens.util.timer.Timer;

namespace BurningKnight.ui {
	public class UiDescriptionBanner : UiString {
		private static UiDescriptionBanner last;
		
		private string title;
		private int titleWidth;
		
		public UiDescriptionBanner() : base(Font.Small) {
			if (last != null) {
				last.Done = true;
			}
			
			last = this;
		}

		public void Show(Item item) {
			Tint = Color.White;
			Tint.A = 0;

			Tween.To(255, 0, x => Tint.A = (byte) x, 0.4f);

			
			title = item.Name;
			Label = item.Description;
			Depth = 3;

			var size = Font.Small.MeasureString(Label);

			Width = size.Width;
			Height = size.Height;
			FinishTyping();

			CenterX = Display.UiWidth / 2f;
			Y = 64f;

			titleWidth = (int) Font.Medium.MeasureString(title).Width;

			Timer.Add(() => {
				Tween.To(0, 255, x => Tint.A = (byte) x, 0.3f).OnEnd = () => {
					title = null;
					Done = true;
				};
			}, 3f);
		}

		public override void Destroy() {
			base.Destroy();

			if (last == this) {
				last = null;
			}
		}

		public override void Render() {
			if (title == null || Engine.Instance.State.Paused) {
				return;
			}

			Graphics.Color = Tint;
			Graphics.Print(title, Font.Medium, new Vector2((Display.UiWidth - titleWidth) / 2f, Y - 16));
			Graphics.Color = ColorUtils.WhiteColor;
			base.Render();
		}
	}
}