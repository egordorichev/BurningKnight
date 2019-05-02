using System;
using Lens.util;

namespace BurningKnight.ui.str.@event {
	public class SpeedEvent : GlyphEvent {
		public float Speed = 1f;
		
		public override void Fire(UiString str, Glyph g) {
			str.Speed = Speed;
		}

		public override void Parse(string[] args) {
			if (args.Length >= 2) {
				try {
					float.TryParse(args[1], out Speed);
				} catch (Exception e) {
					Log.Error(e);
				}
			}
		}
	}
}