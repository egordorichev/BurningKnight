using System.Collections.Generic;
using Lens;
using Lens.entity.component;
using Lens.util;

namespace BurningKnight.ui.dialog {
	// todo: dialog registry?
	public class DialogComponent : Component {
		public readonly string Prefix;
		public readonly Dictionary<string, Dialog> Dialogs = new Dictionary<string, Dialog>();
		public UiDialog Dialog;

		private Dialog currentData;
		
		public DialogComponent(string prefix, params Dialog[] dialogs) {
			Prefix = $"{prefix}_";

			foreach (var d in dialogs) {
				Add(d);
			}
		}

		public void Start(string id) {
			id = $"{Prefix}{id}";
			
			if (!Dialogs.TryGetValue(id, out var dialog)) {
				Log.Error($"Unknown dialog {id}");
				return;
			}
			
			if (Dialog == null) {
				Dialog = new UiDialog();
				Engine.Instance.State.Ui.Add(Dialog);

				Dialog.Owner = Entity;
			}
			
			currentData = dialog;
			Dialog.Say(id);
		}

		public void Add(Dialog d) {
			Dialogs[$"{Prefix}{d.Id}"] = d;
		}
	}
}