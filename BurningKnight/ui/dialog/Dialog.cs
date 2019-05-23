using BurningKnight.assets;
using Lens.util.math;

namespace BurningKnight.ui.dialog {
	public class Dialog {
		public readonly string Id;
		public string[] Next;
		
		public Dialog(string id, string[] next = null) {
			Id = id;
			Next = next;
		}

		public virtual string DecideNext() {
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