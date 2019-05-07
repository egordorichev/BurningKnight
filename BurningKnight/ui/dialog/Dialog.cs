namespace BurningKnight.ui.dialog {
	public class Dialog {
		public readonly string Id;
		public string Next;
		
		public Dialog(string id, string next = null) {
			Id = id;
			Next = next;
		}

		public virtual string DecideNext() {
			return Next;
		}
	}
}