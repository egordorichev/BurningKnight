using System;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiError : FrameRenderer {
		private const int Padding = 6;
		
		private string title;
		private string description;
		private TextureRegion icon;
		private TextureRegion iconBg;

		private Vector2 bgOffset;
		private Vector2 iconOffset;
		private Vector2 titleOffset;
		private Vector2 descriptionOffset;

		public UiError(string t, string desc) {
			title = t;
			description = desc;
			icon = Animations.Get("items").GetSlice("bk:no_hat");
			iconBg = CommonAse.Ui.GetSlice("er_bg");

			var titleSize = Font.Small.MeasureString(title);
			var descriptionSize = Font.Small.MeasureString(description);

			bgOffset = new Vector2(Padding);
			iconOffset = new Vector2(Padding + (20 - icon.Width) / 2f, Padding + (20 - icon.Height) / 2f);
			titleOffset = new Vector2(Padding * 2 + 20, Padding - 1);
			descriptionOffset = new Vector2(Padding * 2 + 20, Padding + 10);

			Width = Math.Max(128, (Math.Max(titleSize.Width, descriptionSize.Width) + 22 + Padding * 3));
			Height = 20 + Padding * 2;
			Depth = 32;

			X = Display.UiWidth - Width - 8;
		}
		
		public override void Init() {
			base.Init();
			Setup("ui", "er_");
		}

		public override void PostInit() {
			base.PostInit();

			Tween.To(Display.UiHeight - 8 - Height, Y, x => Y = x, 0.7f, Ease.BackOut).OnEnd = () => {
				var t = Tween.To(Display.UiHeight + 2, Y, x => Y = x, 0.5f, Ease.BackIn);

				t.Delay = 5f;
				t.OnStart = () => {
					Depth--;
				};
				
				t.OnEnd = () => {
					Done = true;
				};
			};
		}

		public override void Render() {
			base.Render();

			Graphics.Render(iconBg, Position + bgOffset);
			Graphics.Render(icon, Position + iconOffset);
			Graphics.Print(title, Font.Small, Position + titleOffset);
			Graphics.Print(description, Font.Small, Position + descriptionOffset);
		}
	}
}