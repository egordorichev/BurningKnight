namespace BurningKnight.ui.str.@event {
	public class UserEvent : GlyphEvent {
		public string Id = "unknown";
		
		public override void Fire(UiString str, Glyph g) {
			str.EventFired?.Invoke(str, Id);
		}

		public override void Parse(string[] args) {
			if (args.Length >= 2) {
				Id = args[1];
			}
		}
	}
}