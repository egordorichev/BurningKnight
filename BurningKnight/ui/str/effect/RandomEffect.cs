using System;
using Lens;
using Lens.util.math;

namespace BurningKnight.ui.str.effect {
	public class RandomEffect : GlyphEffect {
		public override void Apply(Glyph glyph, int i) {
			var t = Engine.Time;
			
			if (Math.Cos(i * 0.1f + t * 4 * Math.Sin(t * 0.5f) * Math.Cos(t * i) + Math.Tan(t + i * 0.2f)) > 0.6f) {
				glyph.G.Character = 'a';
				glyph.G.Position += Rnd.Vector(-1, 1);
			}
		}
	}
}