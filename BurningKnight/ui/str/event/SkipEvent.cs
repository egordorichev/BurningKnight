namespace BurningKnight.ui.str.@event {
	public class SkipEvent : GlyphEvent {
		public override void Fire(UiString str, Glyph g) {
			str.FinishTyping();
		}
	}
}