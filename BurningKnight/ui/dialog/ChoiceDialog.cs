using System;
using System.Text;
using Lens.assets;

namespace BurningKnight.ui.dialog {
	public class ChoiceDialog : Dialog {
		public readonly string[] Options;
		public readonly string[] Branches;
		public Action<string, int> Callback;

		public int Choice;
		
		public ChoiceDialog(string id, string[] options, string[] branches, Action<string, int> callback = null) : base(id) {
			Options = options;
			Branches = branches;
			Callback = callback;
		}

		public override string Modify(string dialog) {
			var builder = new StringBuilder();
			var i = 0;

			builder.Append(dialog).Append("[sp 10]\n");

			foreach (var option in Options) {
				builder.Append($"[rn {i}]  ").Append(Locale.Get(option)).Append('\n');
				i++;
			}
			
			return builder.ToString();
		}

		public override string DecideNext() {
			var option = Branches[Choice];
			Callback(option, Choice);
			Choice = 0;
			
			return option;
		}
	}
}