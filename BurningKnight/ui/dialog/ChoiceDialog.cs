using System;

namespace BurningKnight.ui.dialog {
	public class ChoiceDialog : Dialog {
		public readonly string[] Options;
		public readonly string[] Branches;
		public Action<string> Callback;

		protected int Choice;
		
		public ChoiceDialog(string id, string[] options, string[] branches, Action<string> callback = null) : base(id) {
			Options = options;
			Branches = branches;
			Callback = callback;
		}

		// todo: implement choice selection
		public override string DecideNext() {
			return Branches[Choice];
		}
	}
}