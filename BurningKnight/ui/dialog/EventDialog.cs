using System;

namespace BurningKnight.ui.dialog {
	public class EventDialog : Dialog {
		public readonly Func<string> Callback;
		
		public EventDialog(string id, Func<string> callback) : base(id) {
			Callback = callback;
		}

		public override string DecideNext() {
			return Callback();
		}
	}
}