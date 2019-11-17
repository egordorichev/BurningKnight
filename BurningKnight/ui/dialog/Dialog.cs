using System;
using System.Collections.Generic;
using BurningKnight.assets;
using Lens.util.math;

namespace BurningKnight.ui.dialog {
	public class Dialog {
		public readonly string Id;
		public string[] Next;

		public List<Func<Dialog, DialogComponent, Dialog>> Callbacks = new List<Func<Dialog, DialogComponent, Dialog>>();
		
		public Dialog(string id, string[] next = null) {
			Id = id;
			Next = next;
		}

		public virtual string DecideNext() {
			if (Next == null || Next.Length == 0) {
				return null;
			}
			
			return Next?[Rnd.Int(Next.Length)];
		}

		public virtual Dialog GetNext() {
			var next = DecideNext();
			return next != null ? Dialogs.Get(next) : null;
		}

		public virtual string Modify(string dialog) {
			return dialog;
		}

		public virtual void Reset() {
			
		}
	}
}