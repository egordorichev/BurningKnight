namespace BurningKnight.ui.str.effect {
	public class BoldEffect : GlyphEffect {
		public float Scale = 0.2f;
		
		public override void Apply(Glyph glyph, int i) {
			glyph.Scale.X += Scale;
			glyph.Scale.Y += Scale;
		}
	}
}