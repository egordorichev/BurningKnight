using BurningKnight.assets;
using Lens.assets;
using Lens.graphics;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.ui.inventory {
	/*
	 * TODO: support rerolling and removing
	 */
	public class UiItem : UiEntity {
		public static UiItem Hovered;

		private string id;

		public string Id {
			get => id;

			set {
				id = value;

				if (id == null) {
					return;
				}
				
				Name = Locale.Get(id);
				Description = Locale.Get($"{id}_desc");
				region = CommonAse.Items.GetSlice(id);

				NameSize = Font.Small.MeasureString(Name);
				DescriptionSize = Font.Small.MeasureString(Description);

				Width = region.Width;
			}
		}

		private TextureRegion region;
		public string Name;
		public string Description;
		public Vector2 NameSize;
		public Vector2 DescriptionSize;
		
		private string countStr;
		private int count;
		private int countW;
		private int countH;

		private float border;
		public bool DrawBorder;

		public Vector2 IconScale = Vector2.One;

		public int Count {
			get => count;

			set {
				count = value;
				countStr = $"{count}";
				
				var s = Font.Small.MeasureString(countStr);
				countW = (int) s.Width;
				countH = (int) s.Height;

				IconScale.X = 0;
				IconScale.Y = 2;

				Tween.To(1, IconScale.X, x => IconScale.X = x, 0.3f);
				Tween.To(1, IconScale.Y, x => IconScale.Y = x, 0.3f);
			}
		}

		public UiItem() {
			Count = 1;
			ScaleMod = 2;
		}

		public override void Destroy() {
			base.Destroy();

			if (Hovered == this) {
				Hovered = null;
			}
		}

		protected override void OnHover() {
			if (id == null) {
				return;
			}
			
			base.OnHover();
			
			Tween.To(1, border, x => border = x, 0.3f);
			Audio.PlaySfx("moving", 0.5f);

			Hovered = this;
		}

		protected override void OnUnhover() {
			if (id == null) {
				return;
			}
			
			base.OnUnhover();
			
			Tween.To(0, border, x => border = x, 0.3f);

			if (Hovered == this) {
				Hovered = null;
			}
		}

		public override void Render() {
			if (id == null) {
				return;
			}

			var b = DrawBorder ? 1 : border;
			
			if (b > 0.01f) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(b);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(region, Center + d, 0, region.Center, IconScale * scale);
				}

				Shaders.End();
			}
			
			Graphics.Render(region, Center, 0, region.Center, IconScale * scale);
			
			if (count < 2) {
				return;
			}
			
			Graphics.Print(countStr, Font.Small, (int) (X + Width - countW), (int) (Y + Height + 6 - countH));
		}
	}
}