using BurningKnight.assets;
using Lens;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiBanner : FrameRenderer {
		private string label;
		private float labelX;
		private float targetW;
		private Color color = Color.White;
		
		public UiBanner(string str) {
			var w = Font.Small.MeasureString(str).Width;
			
			label = str;
			labelX = w * -0.5f;

			targetW = w + 28;

			Width = 20;
			Height = 23;

			color.A = 0;
		}
		
		public override void Init() {
			base.Init();
			
			Setup("ui", "scroll_");

			CenterX = (Display.UiWidth) / 2f;
			CenterY = Display.UiHeight + 64;

			// fix centring
			Tween.To(Display.UiHeight - 64f, CenterY, x => CenterY = x, 1f, Ease.BackOut).OnEnd = () => {
				Tween.To(targetW, Width, x => {
					Width = x;
					CenterX = (Display.UiWidth) / 2f;
				}, 0.7f, Ease.BackOut).OnEnd = () => {
					Tween.To(255, 0, x => color.A = (byte) x, 0.3f);

					var t = Tween.To(0, 255, x => color.A = (byte) x, 0.3f);
					t.Delay = 3;

					t.OnEnd = () => {
						Tween.To(20, Width, x => {
								Width = x;
								CenterX = (Display.UiWidth) / 2f;
							}, 0.7f, Ease.BackIn).OnEnd = 
							() => Tween.To(Display.UiHeight + 64, CenterY, x => CenterY = x, 1f, Ease.BackIn).OnEnd = 
								() => { Done = true; };
					};
				};
			};
		}

		public override void Render() {
			base.Render();

			Graphics.Color = color;
			Graphics.Print(label, Font.Small, new Vector2(labelX + Width / 2, 8) + Position);
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}