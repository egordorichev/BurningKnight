using System;
using System.Collections.Generic;
using BurningKnight.assets;
using Random = Lens.util.math.Random;

namespace BurningKnight.ui.dialog {
	public class Dialog {
		public readonly string Id;
		public string[] Next;

		public List<Action<Dialog>> Callbacks = new List<Action<Dialog>>();
		
		public Dialog(string id, string[] next = null) {
			Id = id;
			Next = next;
		}

		public virtual string DecideNext() {
			if (Next == null || Next.Length == 0) {
				return null;
			}
			
			return Next?[Random.Int(Next.Length)];
		}

		public virtual Dialog GetNext() {
			var next = DecideNext();
			return next != null ? Dialogs.Get(next) : null;
		}

		public virtual string Modify(string dialog) {
			return dialog;
		}
	}
}