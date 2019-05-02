using Lens.graphics;

namespace BurningKnight.ui.str.effect {
	public class RainbowEffect : GlyphEffect {
		public float Speed = 1f;
		public float Width = 0.05f;
		
		public override void Apply(Glyph glyph, int i) {
			glyph.Color = ColorUtils.FromHSV(((Time * Speed + i * Width) * 360f) % 360f, 100f, 100f);
		}
	}
}