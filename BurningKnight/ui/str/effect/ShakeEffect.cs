using Lens.util;

namespace BurningKnight.ui.str.effect {
	public class ShakeEffect : GlyphEffect {
		public float Amplitude = 0.01f;
		
		public override void Apply(Glyph glyph, int i) {
			var t = Time * 6f + i * 10;
			glyph.Offset.X = Noise.Generate(-t) * Amplitude;
			glyph.Offset.Y = Noise.Generate(-t * 0.95f + 32f) * Amplitude;
		}
	}
}