namespace BurningKnight.ui.str.@event {
	public class DelayEvent : GlyphEvent {
		public float Delay = 0.5f;
		
		public override void Fire(UiString str, Glyph g) {
			str.Delay += Delay;
		}
	}
}