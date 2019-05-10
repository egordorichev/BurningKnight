using System.Collections.Generic;
using BurningKnight.ui.dialog;
using Lens.assets;
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
			// Ord
			Add(new CombineDialog("ord_link", "ord_info"));
			Add(new CombineDialog("ord_info"));
		}

		public static Dialog Get(string id) {
			if (!dialogs.TryGetValue(id, out var dialog)) {
				if (Locale.Contains(id)) {
					dialog = new Dialog(id);
					dialogs[id] = dialog;
				} else {
					Log.Error($"Unknown dialog {id}");
					return null;
				}
			}

			return dialog;
		}
	}
}