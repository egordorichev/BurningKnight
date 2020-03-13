using System;
using BurningKnight.assets;
using Lens;
using Lens.util.math;

namespace BurningKnight.ui.str.effect {
	public class RandomEffect : GlyphEffect {
		public override void Apply(Glyph glyph, int i) {
			var t = Engine.Time * 0.1f;
			var a = Math.Cos(i * 0.1f + t * 4 * Math.Sin(t * 0.5f) * Math.Cos(t * i)) > 0.6f;
			
			if (glyph.State != a) {
				glyph.State = a;
				glyph.G.FontRegion = Font.Small.GetCharacterRegion(a ? Rnd.Int(255) : glyph.G.Character);
			}
		}
	}
}