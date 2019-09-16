using BurningKnight.assets;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui.str.effect {
	public class ColorEffect : GlyphEffect {
		public Color Color = ColorUtils.WhiteColor;

		public override void Apply(Glyph glyph, int i) {
			glyph.Color = Color;
		}

		public void ParseColor(string color) {
			if (color.StartsWith("#")) {
				Color = ColorUtils.FromHex(color);
			} else if (Palette.Colors.TryGetValue(color, out var c)) {
				Color = c;
			} else {
				Log.Error($"Unknown color {color}");
			}
		}
	}
}