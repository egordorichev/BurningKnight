using System;
using BurningKnight.assets;
using BurningKnight.assets.achievements;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiAchievement : FrameRenderer {
		public static UiAchievement Current;
		
		private const int Padding = 4;
		
		private string title;
		private string description;
		private TextureRegion icon;

		private Vector2 iconOffset;
		private Vector2 titleOffset;
		private Vector2 descriptionOffset;
		
		public UiAchievement(string id) {
			Current = this;
			
			title = Locale.Get($"ach_{id}");
			description = Locale.Get($"ach_{id}_desc");
			icon = Animations.Get("achievements").GetSlice(id);

			var titleSize = Font.Small.MeasureString(title);
			var descriptionSize = Font.Small.MeasureString(description);
			
			iconOffset = new Vector2(Padding);
			titleOffset = new Vector2(Padding * 2 + icon.Width, Padding);
			descriptionOffset = new Vector2(Padding * 2 + icon.Width, Padding + 10);

			Width = Math.Max(128, (Math.Max(titleSize.Width, descriptionSize.Width) + icon.Width + Padding * 3));
			Height = icon.Height + Padding * 2;
		}
		
		public override void Init() {
			base.Init();
			
			Setup("ui", "bg_");
			Audio.PlaySfx("ui_achievement");
		}

		public override void Destroy() {
			base.Destroy();

			if (Current == this) {
				Current = null;
			}
		}

		public override void PostInit() {
			base.PostInit();

			Tween.To(Display.UiHeight - 8 - Height, Y, x => Y = x, 1f, Ease.BackOut).OnEnd = () => {
				var t = Tween.To(Display.UiHeight + 2, Y, x => Y = x, 0.5f, Ease.BackIn);

				t.Delay = 5f;
				t.OnStart = () => {
					Current = null;
					Achievements.AchievementBuffer.RemoveAt(0);
				};
				
				t.OnEnd = () => {
					Done = true;
				};
			};
		}

		public override void Render() {
			base.Render();
			
			Graphics.Render(icon, Position + iconOffset);
			Graphics.Print(title, Font.Small, Position + titleOffset);
			Graphics.Print(description, Font.Small, Position + descriptionOffset);
		}
	}
}