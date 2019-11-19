using System;

namespace BurningKnight.ui.str.effect {
	public class WaveEffect : GlyphEffect {
		public float Amplitude = 1f;
		public float Width = 1f;
		public float Speed = 1f;
		
		public override void Apply(Glyph glyph, int i) {
			glyph.Offset.Y += (float) Math.Cos(Time * Speed * 4f + i * Math.PI / 8f * Width) * Amplitude * 2.5f;
		}
	}
}