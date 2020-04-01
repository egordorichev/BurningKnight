using BurningKnight.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiTableEntry : UiLabel {
		private const float XPadding = 3;
		public const float YPadding = 1;
		
		private TextureRegion texture;
		private string value;
		private float valueWidth;

		public Color Color = ColorUtils.WhiteColor;

		public string Value {
			set {
				this.value = value;

				valueWidth = Font.MeasureString(this.value).Width;
			}
		}

		public override void AddComponents() {
			base.AddComponents();

			texture = CommonAse.Ui.GetSlice("table_item");
			Width = texture.Width;
			Height = texture.Height;

			Font = assets.Font.Small;
		}

		public override void Render() {
			Graphics.Color.A = 200;
			Graphics.Render(texture, Position);
			Graphics.Color.A = 255;

			Graphics.Color = Color;
			Graphics.Print(label, Font, new Vector2(X + XPadding, Y + YPadding));
			Graphics.Print(value, Font, new Vector2(Right - XPadding - valueWidth, Y + YPadding));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}