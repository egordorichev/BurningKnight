using BurningKnight.assets;
using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace BurningKnight.ui {
	public class UiLabel : UiEntity {
		protected string label;
		public BitmapFont Font = assets.Font.Medium;
		
		public string Label {
			get => label;

			set {
				if (label != value) {
					label = value;
					
					var size = Font.MeasureString(label);

					Width = size.Width;
					Height = size.Height;
					
					origin = new Vector2(Width / 2, Height / 2);
				}
			}
		}

		public string LocaleLabel {
			set => Label = Locale.Get(value);
		}

		public override void Render() {
			Graphics.Print(label, Font, Position + origin, angle, origin, new Vector2(scale));
		}
	}
}