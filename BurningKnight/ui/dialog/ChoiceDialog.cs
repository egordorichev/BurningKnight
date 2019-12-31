using System;
using System.Collections.Generic;
using System.Text;
using Lens.assets;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.ui.dialog {
	public class ChoiceDialog : Dialog {
		public readonly string[] Options;
		public readonly List<string[]> Branches;
		public Action<string, int> Callback;

		public int Choice;
		public int Last;
		
		public ChoiceDialog(string id, string[] options, List<string[]> branches, Action<string, int> callback = null) : base(id) {
			Options = options;
			Branches = branches;
			Callback = callback;
		}

		public override string Modify(string dialog) {
			var builder = new StringBuilder();
			var i = 0;

			builder.Append(dialog).Append("\n");

			foreach (var option in Options) {
				builder.Append($"[rn {i}]  ").Append(Locale.Get(option));
				i++;

				if (i < Options.Length) {
					builder.Append('\n');
				}
			}

			return builder.ToString();
		}

		public override string DecideNext() {
			if (Branches.Count == 0) {
				return null;
			}
			
			var array = Branches[Choice];
			var option = array.Length == 0 ? null : array[Rnd.Int(array.Length)]; 
			
			Callback?.Invoke(option, Choice);
			
			Last = Choice;
			Choice = 0;
			
			return option;
		}
	}
}