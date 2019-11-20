namespace BurningKnight.ui.str.effect {
	public class FlipEffect : GlyphEffect {
		public override void Apply(Glyph glyph, int i) {
			glyph.Scale.Y *= -1;
		}
	}
}