using System;

namespace BurningKnight.ui.str.effect {
	public class BlinkEffect : GlyphEffect {
		public override void Apply(Glyph glyph, int i) {
			glyph.Color.A = (byte) Math.Round((Math.Cos(Time * 2f) * 0.5f + 0.5f) * 255);
		}
	}
}