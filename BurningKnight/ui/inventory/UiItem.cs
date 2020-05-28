using System.Linq;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BurningKnight.ui.inventory {
	public class UiItem : UiEntity {
		public static UiItem Hovered;

		private string id;
		public float TextA;

		public string Id {
			get => id;

			set {
				id = value;

				if (id == null) {
					Region = null;
					return;
				}
				
				var idd = Scourge.IsEnabled(Scourge.OfEgg)
					? Items.Datas.Values.ElementAt(Rnd.Int(Items.Datas.Count)).Id
					: Id;
				
				Name = Locale.Get(idd);
				Description = Locale.Get($"{idd}_desc");
				Region = CommonAse.Items.GetSlice(id);

				NameSize = Font.Small.MeasureString(Name);
				DescriptionSize = Font.Small.MeasureString(Description);

				Width = Region.Width;
			}
		}

		public string Name;
		public string Description;
		public Vector2 NameSize;
		public Vector2 DescriptionSize;
		public bool OnTop;

		public TextureRegion Region;
		private string countStr;
		private int count;
		private int countW;
		private int countH;

		private float border;
		public bool DrawBorder;
		public bool Scourged;

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
			Tween.To(1, TextA, x => TextA = x, 0.3f);

			if (Settings.UiSfx) {
				Audio.PlaySfx("ui_moving", 0.5f);
			}

			Hovered = this;
		}

		protected override void OnUnhover() {
			if (id == null) {
				return;
			}
			
			base.OnUnhover();
			
			Tween.To(0, border, x => border = x, 0.3f);
			Tween.To(0, TextA, x => TextA = x, 0.3f);

			if (Hovered == this) {
				Hovered = null;
			}
		}

		public override void Render() {
			if (id == null) {
				return;
			}

			var b = DrawBorder ? 1 : border;
			
			if (Scourged || b > 0.01f) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(Scourged ? 1 : b);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(Scourged ? ItemGraphicsComponent.ScourgedColor : ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(Region, Center + d, 0, Region.Center, IconScale * scale);
				}

				Shaders.End();
			}
			
			Graphics.Render(Region, Center, 0, Region.Center, IconScale * scale);
			
			if (count < 2) {
				return;
			}

			Graphics.Print(countStr, Font.Small, (int) (X + Width - countW), (int) (Y + Height + 6 - countH));
		}

		public override void OnClick() {
			base.OnClick();

			if (Input.Keyboard.IsDown(Keys.LeftControl) || Input.Keyboard.IsDown(Keys.LeftShift) ||
			    Input.Keyboard.IsDown(Keys.RightControl)) {

				System.Diagnostics.Process.Start($"https://wiki.burningknight.net/?item={Id}");
			}
		}
	}
}