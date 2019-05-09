using System.Collections.Generic;
using BurningKnight.ui.dialog;
using Lens.util;

namespace BurningKnight.assets {
	public static class Dialogs {
		private static Dictionary<string, Dialog> dialogs = new Dictionary<string, Dialog>();

		public static void Add(Dialog dialog) {
			dialogs[dialog.Id] = dialog;
		}

		static Dialogs() {
			AddNpc();
		}
		
		private static void AddNpc() {
			// Old Man
			Add(new Dialog("om_hello", "om_best"));
			Add(new EventDialog("om_best", () => "om_test"));

			Add(new ChoiceDialog("om_test", new[] {
				"om_a", "om_b"
			}, new[] {
				"om_awesome", "om_eh"
			}, (c, i) => {
				Log.Debug($"{c} was selected (#{i})");
			}));
			
			Add(new Dialog("om_awesome"));
			Add(new Dialog("om_eh"));
		}

		public static Dialog Get(string id) {
			if (!dialogs.TryGetValue(id, out var dialog)) {
				Log.Error($"Unknown dialog {id}");
			}

			return dialog;
		}
	}
}