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
			Add(new CombineDialog("ord_link", "ord_info", "ord_internet"));
			Add(new CombineDialog("ord_info", "ord_interested", "ord_interested"));
			Add(new CombineDialog("ord_interested", "ord_marketing"));
			Add(new CombineDialog("ord_marketing", "ord_trailer", "ord_steam"));
			Add(new CombineDialog("ord_internet"));
			Add(new Dialog("ord_trailer"));
			Add(new Dialog("ord_steam"));
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