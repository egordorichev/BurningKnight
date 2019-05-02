using System;

namespace BurningKnight.ui.str.effect {
	public class ItalicEffect : GlyphEffect {
		public override void Apply(Glyph glyph, int i) {
			glyph.Angle = (float) (Math.PI * 0.06f);
		}
	}
}